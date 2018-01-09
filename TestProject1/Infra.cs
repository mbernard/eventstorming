using System;
using System.Collections.Generic;
using System.Linq;
using Infra;
using NUnit.Framework;

namespace TestProject1
{
    [TestFixture]
    public class Infra
    {
        [Test]
        public void ExecuteWorks()
        {
            var events = new List<object>();
            var commandExecutor = new CommandExecutor(new Action<object>[]
            {
                events.Add
            });
            var command = new DoSomething();
            commandExecutor.Execute(new Something(), command);
            
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
        public string Id { get; }

        public Something()
        {
            Id = Guid.NewGuid().ToString();
        }
        
        public void Hydrate(object @event)
        {
            if (@event is DidSomething)
            {
                OnDidSomething((DidSomething)@event);
            }
        }

        private void OnDidSomething(DidSomething @event)
        {
            somethingDone = true;
        }

        public IEnumerable<object> DoSomething(DoSomething command)
        {
            return new[] {new DidSomething()};
        }

        public IEnumerable<object> Execute(object command)
        {
            if (command is DoSomething)
            {
                return DoSomething((DoSomething) command);
            }
            throw new InvalidOperationException();        }
    }

    public class DidSomething
    {
    }
}