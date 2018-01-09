using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Infra;
using NUnit.Framework;

namespace TestProject1
{
    [TestFixture]
    public class Infra
    {
        private object OrderSubmittedMapper(object @event)
        {
            var orderSubmitted = @event as OrderSubmitted;
            if (orderSubmitted != null)
                return new OrderSubmitted_V2
                {
                    Address = orderSubmitted.Address,
                    Date = orderSubmitted.Date,
                    OrderId = orderSubmitted.OrderId
                };

            return @event;
        }

        [Test]
        public void ExecuteWorks()
        {
            var events = new List<object>();
            var commandExecutor = new CommandExecutor(new Action<object>[]
                {
                    events.Add
                },
                new List<Func<object, object>> {OrderSubmittedMapper});
            var command = new DoSomething();
            commandExecutor.Execute<Something>(Guid.NewGuid().ToString(), command);

            Assert.True(events.Count == 1);
            Assert.True(events.First() is DidSomething);
        }
    }

    public class DoSomething
    {
    }

    public class Something : IAggregate
    {
        private bool somethingDone;

        public Something()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; }

        public void Hydrate(object @event)
        {
            if (@event is DidSomething)
                OnDidSomething((DidSomething) @event);
        }

        public IEnumerable<object> Execute(object command)
        {
            if (command is DoSomething)
                return DoSomething((DoSomething) command);
            throw new InvalidOperationException();
        }

        private void OnDidSomething(DidSomething @event)
        {
            somethingDone = true;
        }

        public IEnumerable<object> DoSomething(DoSomething command)
        {
            return new[] {new DidSomething()};
        }
    }

    public class DidSomething
    {
    }
}