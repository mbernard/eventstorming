using System;
using System.Collections.Generic;
using System.Linq;

namespace Infra
{
    public class InMemoryEventStore
    {
        private IDictionary<string, List<object>> streams = new Dictionary<string, List<object>>();


        public IEnumerable<object> ReadEvents(string id)
        {
            return streams.TryGetValue(id, out var stream) ? stream : Enumerable.Empty<object>();
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
        private readonly InMemoryEventStore eventStore = new InMemoryEventStore();
        private readonly IEnumerable<Action<object>> handlers;

        public CommandExecutor(IEnumerable<Action<object>> handlers)
        {
            this.handlers = handlers;
        }

        public void Execute<TCommand>(IAggregate aggregate, TCommand command)
        {
            var events = eventStore.ReadEvents(aggregate.Id);
            foreach (var @event in events) aggregate.Hydrate(@event);

            var newEvents = aggregate.Execute(command);
            eventStore.SaveEvents(aggregate.Id, newEvents);
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