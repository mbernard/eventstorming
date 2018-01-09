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
                throw new Exception("Order not received.");
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

    public class CancelOrder
    {
    }
}