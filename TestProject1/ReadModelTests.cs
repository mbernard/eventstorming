﻿using System;
using System.Linq;
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
            outstandingOrders.Apply(new ItemAddedToOrder {OrderId = "3", Name = "Pizza", Price = 2});
            outstandingOrders.Apply(new ItemAddedToOrder {OrderId = "3", Name = "Burger", Price = 1});
            outstandingOrders.Apply(new OrderSubmitted {OrderId = "5", Date = DateTime.Now});
            outstandingOrders.Apply(new ItemAddedToOrder {OrderId = "5", Name = "Chicken Wings", Price = 1});
            outstandingOrders.Apply(new ItemAddedToOrder {OrderId = "5", Name = "Poutine", Price = 2});
            
            //When
            outstandingOrders.Apply(new OrderCanceled {OrderId = "3"});
            
            //Then
            Assert.True(outstandingOrders.Orders.Count() == 1);
            var firstOrder = outstandingOrders.Orders.First();
            Assert.True(firstOrder.Key == "5");
            Assert.True(firstOrder.Value.Status == "NotStarted");
            var firstOrderItems = firstOrder.Value.Items;
            Assert.True(firstOrderItems.First() == "Chicken Wings");
            Assert.True(firstOrderItems.ElementAt(1) == "Poutine");
        }
        
    }
}