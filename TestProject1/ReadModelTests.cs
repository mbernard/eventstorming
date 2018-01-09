using System;
using Domain;
using NUnit.Framework;

namespace TestProject1
{
    [TestFixture]
    public class ReadModelTests
    {
        [Test]
        public void OutstandingOrders_When_CancelOrderBeforeCooking()
        {
            //Given
            var outstandingOrders = new OutstandingOrders();
            outstandingOrders.Apply(new OrderSubmitted {OrderId = "3", Date = DateTime.Now});
            outstandingOrders.Apply(new OrderSubmitted {OrderId = "5", Date = DateTime.Now});
            
            //When
            outstandingOrders.Apply(new OrderCanceled {OrderId = "3"});
            
            //Then
            Assert.True(outstandingOrders.Orders.Count() == 1);
            Assert.True(outstandingOrders.Orders.First().OrderId == "5");
            Assert.True(outstandingOrders.Orders.First().Status == "NotStarted");
            Assert.True(outstandingOrders.Orders.First().Items.First().Name == "Chicken Wings");
            Assert.True(outstandingOrders.Orders.First().Items.ElementAt(1).Name == "Poutine");
        }
        
    }
}