﻿using System;
using System.Collections.Generic;
using System.Linq;

using Domain;

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
            order.Hydrate(new OrderSubmitted());

            //When
            var events = order.Execute(new CancelOrder("1"));

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
                events = order.Execute(new CancelOrder("1"));
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
		
		[Test]
        public void TotalPriceIsCorrect()
        {
            // Given
            // When
            var ordersPersUser = new OrderPerUser();
            ordersPersUser.Apply(new ItemAddedToOrder {Name = "Item1", Price = 12.34M});
            ordersPersUser.Apply(new ItemAddedToOrder {Name = "Item2", Price = 34.56M});
            ordersPersUser.Apply(new OrderSubmitted {Date = new DateTime(2010, 10, 10)});

            // Then
            Assert.AreEqual(46.90M, ordersPersUser.TotalPrice);
        }

        [Test]
        public void CannotCancelPickedUpOrder()
        {
            var order = new Order();
            order.Hydrate(new OrderSubmitted());
            order.Hydrate(new OrderPickedUp());

            Assert.Catch<Exception>(() => order.Execute(new CancelOrder("1")));
        }

        [Test]
        public void StartFoodPrep()
        {
            var order = new Order();
            order.Hydrate(new OrderSubmitted());

            var events = order.Execute(new StartFoodPreparation());

            Assert.True(events.Count() == 1);
            Assert.True(events.First() is OrderStarted);
        }
    }
}