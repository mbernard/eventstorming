using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace TestProject1
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void CancelAnOrder()
        {
            //Given
            var order = new Order();
            order.Hydrate(new OrderReceived());
            
            //When
            var events = order.Execute(new CancelOrder());
            
            //Then
            Assert.True(events.Count() == 1);
            Assert.True(events.First() is OrderCanceled);
        }

        [Test]
        public void CancelANonReceivedOrder()
        {
            //Given
            var order = new Order();
            
            //When
            IEnumerable<object> events = Enumerable.Empty<object>();
            Exception caught = null;
            try
            {
                events = order.Execute(new CancelOrder());
            }
            catch (Exception e)
            {
                caught = e;
            }

            //Then
            Assert.True(!events.Any());
            Assert.True(caught != null);
            Assert.True(caught is OrderNotReceivedException);
        }
        
    }

    public class OrderCanceled
    {
        
    }

    public class OrderReceived
    {
        
    }

    public class Order
    {
        private bool received;

        public IEnumerable<object> Execute(object order)
        {
            if (order is CancelOrder)
            {
                return CancelOrder((CancelOrder) order);
            }
            throw new InvalidOperationException("Unknown command.");
        }

        private IEnumerable<object> CancelOrder(CancelOrder cancelOrder)
        {
            if (!received)
            {
                throw new OrderNotReceivedException("Order not received.");
            }

            return new[] {new OrderCanceled()};
        }

        public void Hydrate(object @event)
        {
            if (@event is OrderReceived)
            {
                OnOrderReceived((OrderReceived) @event);
            }
        }

        private void OnOrderReceived(OrderReceived @event)
        {
            this.received = true;
        }
    }

    public class OrderNotReceivedException : Exception
    {
        public OrderNotReceivedException(string message) : base(message)
        {}
    }

    public class CancelOrder
    {
    }
}