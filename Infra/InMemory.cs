using System;
using System.Collections.Generic;
using System.Linq;

namespace Infra
{
    public class InMemoryEventStore
    {
        private IDictionary<string, List<object>> streams = new Dictionary<string, List<object>>();
        private IEnumerable<Func<object, object>> eventMappers;

        public InMemoryEventStore(IEnumerable<Func<object, object>> eventMappers)
        {
            this.eventMappers = eventMappers;
        }

        public IEnumerable<object> ReadEvents(string id)
        {
            var s =  streams.TryGetValue(id, out var stream) ? stream : Enumerable.Empty<object>();

            return s.Select(x =>eventMappers.Aggregate(x, (e, f) => f(e)));
        }

        public void SaveEvents(string id, IEnumerable<object> newEvents)
        {
            if (!streams.TryGetValue(id, out var existing))
            {
                existing = new List<object>();
                streams[id] = existing;
            }
            existing.AddRange(newEvents);
        }
    }

    public class CommandExecutor
    {
        private readonly InMemoryEventStore eventStore;
        private readonly IEnumerable<Action<object>> handlers;

        public CommandExecutor(IEnumerable<Action<object>> handlers, IEnumerable<Func<object, object>> eventMappers)
        {
            this.handlers = handlers;
            this.eventStore =  new InMemoryEventStore(eventMappers);
        }

        public void Execute<TAggregate>(string aggregateId, object command) where TAggregate : IAggregate, new()
        {
            if (string.IsNullOrEmpty(aggregateId)) throw new ArgumentException("aggregateId is missing.");
            
            var aggregate = new TAggregate();
            var events = eventStore.ReadEvents(aggregateId);
            foreach (var @event in events) aggregate.Hydrate(@event);

            var newEvents = aggregate.Execute(command);
            eventStore.SaveEvents(aggregateId, newEvents);
            Publish(newEvents);
        }

        private void Publish(IEnumerable<object> newEvents)
        {
            foreach (var @event in newEvents)
            {
                foreach (var handler in handlers)
                {
                    handler(@event);
                }
            }
        }
    }

    public class InMemoryRepository
    {
    }
}