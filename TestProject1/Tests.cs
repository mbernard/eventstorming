using System;
using System.Linq;
using Domain;
using NUnit.Framework;

namespace TestProject1
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void CancelANonReceivedOrder()
        {
            //Given
            var order = new Order();

            //When
            var events = Enumerable.Empty<object>();
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
            Assert.True(caught is OrderNotSubmittedException);
        }

        [Test]
        public void CancelAnOrder()
        {
            //Given
            var order = new Order();
            order.Hydrate(new OrderSubmitted_V2());

            //When
            var events = order.Execute(new CancelOrder("1"));

            //Then
            Assert.True(events.Count() == 1);
            Assert.True(events.First() is OrderCanceled);
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
        public void FinishAnOrder()
        {
            // Given
            var order = new Order();
            order.Hydrate(new OrderStarted());

            // When
            var events = order.Execute(new FinishOrder());

            // Then
            Assert.AreEqual(1, events.Count());
            Assert.True(events.First() is OrderPrepared);
        }

        [Test]
        public void FinishAnOrderException()
        {
            // Given
            var order = new Order();
            order.Hydrate(new OrderPickedUp());

            // When
            // Then
            Assert.Catch<InvalidOperationException>(()=>order.Execute(new FinishOrder()));
        }

        [Test]
        public void GivenFoodDeliveredThenOrderStatusIsDelivered()
        {
            // Given
            var orderStatus = new GetOrderStatus();
            orderStatus.Apply(new OrderDelivered());

            // Then
            Assert.AreEqual(OrderStatus.Delivered, orderStatus.Status);
        }

        [Test]
        public void GivenOrderInTransitThenOrderStatusIsInTransit()
        {
            // Given
            var orderStatus = new GetOrderStatus();
            orderStatus.Apply(new OrderPickedUp());

            // Then
            Assert.AreEqual(OrderStatus.PickedUp, orderStatus.Status);
        }

        [Test]
        public void GivenOrderPreparedThenOrderStatusIsPrepared()
        {
            // Given
            var orderStatus = new GetOrderStatus();
            orderStatus.Apply(new OrderPrepared());

            // Then
            Assert.AreEqual(OrderStatus.Prepared, orderStatus.Status);
        }

        [Test]
        public void GivenOrderStartedThenOrderStatusIsStarted()
        {
            // Given
            var orderStatus = new GetOrderStatus();
            orderStatus.Apply(new OrderStarted());

            // Then
            Assert.AreEqual(OrderStatus.Started, orderStatus.Status);
        }

        [Test]
        public void GivenOrderSubmittedThenOrderStatusIsSubmitted()
        {
            // Given
            var orderStatus = new GetOrderStatus();
            orderStatus.Apply(new OrderSubmitted());

            // Then
            Assert.AreEqual(OrderStatus.Submitted, orderStatus.Status);
        }

        [Test]
        public void PreparedOrderIsRemovedFromOutstandingOrders()
        {
            // Given
            var outstandingOrders = new OutstandingOrders();
            outstandingOrders.Apply(new ItemAddedToOrder { OrderId = "3", Name = "Hamburger", Price = 5 });
            outstandingOrders.Apply(new ItemAddedToOrder { OrderId = "3", Name = "Pizza", Price = 5 });
            outstandingOrders.Apply(new OrderSubmitted_V2 { OrderId = "3" });
            outstandingOrders.Apply(new ItemAddedToOrder { OrderId = "5", Name = "Chicken Wings", Price = 10 });
            outstandingOrders.Apply(new OrderSubmitted_V2 { OrderId = "5" });
            outstandingOrders.Apply(new OrderPrepared{ OrderId = "5" });

            // When
            // Then
            Assert.AreEqual(1, outstandingOrders._orders.Count);
            Assert.AreEqual("3", outstandingOrders._orders.First().Key);
        }

        [Test]
        public void CanceledOrderIsRemovedFromOutstandingOrders()
        {
            // Given
            var outstandingOrders = new OutstandingOrders();
            outstandingOrders.Apply(new ItemAddedToOrder { OrderId = "3", Name = "Hamburger", Price = 5 });
            outstandingOrders.Apply(new ItemAddedToOrder { OrderId = "3", Name = "Pizza", Price = 5 });
            outstandingOrders.Apply(new OrderSubmitted_V2 { OrderId = "3" });
            outstandingOrders.Apply(new ItemAddedToOrder { OrderId = "5", Name = "Chicken Wings", Price = 10 });
            outstandingOrders.Apply(new OrderSubmitted_V2 { OrderId = "5" });
            outstandingOrders.Apply(new OrderCanceled { OrderId = "5" });

            // When
            // Then
            Assert.AreEqual(1, outstandingOrders._orders.Count);
            Assert.AreEqual("3", outstandingOrders._orders.First().Key);
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

        [Test]
        public void TotalPriceIsCorrect()
        {
            // Given
            // When
            var ordersPersUser = new OrderPerUser();
            ordersPersUser.Apply(new ItemAddedToOrder {Name = "Item1", Price = 12.34M});
            ordersPersUser.Apply(new ItemAddedToOrder {Name = "Item2", Price = 34.56M});
            ordersPersUser.Apply(new OrderSubmitted_V2 { Date = new DateTime(2010, 10, 10)});

            // Then
            Assert.AreEqual(46.90M, ordersPersUser.TotalPrice);
        }

        [Test]
        public void OrderPickedUp()
        {
            var order = new Order();
            order.Hydrate(new OrderSubmitted());
            order.Hydrate(new OrderStarted());
            order.Hydrate(new OrderPrepared());

            var events = order.Execute(new PickUpOrderForDelivery());

            Assert.True(events.Count() == 1);
            Assert.True(events.First() is OrderPickedUp);
        }

        [Test]
        public void ConfirmDelivery()
        {
            // Given
            var order = new Order();
            order.Hydrate(new OrderPrepared());
            order.Hydrate(new OrderPickedUp());
            
            // When
            var events = order.Execute(new ConfirmDelivery());

            // Then
            Assert.True(events.Count() == 1);
            Assert.True(events.First() is OrderDelivered);
        }

        [Test]
        public void OrderCreated()
        {
            var order = new Order();

            var events = order.Execute(new CreateOrder("1"));

            Assert.True(events.Count() == 1);
            Assert.True(events.First() is OrderCreated);
        }

        [Test]
        public void AddItem()
        {
            var order = new Order();
            order.Hydrate(new OrderCreated("1"));

            var events = order.Execute(new AddItemToOrder("Potato", 15.00M));

            Assert.True(events.Count() == 1);
            Assert.True(events.First() is ItemAddedToOrder);
            Assert.True(((ItemAddedToOrder)events.First()).Name == "Potato");
            Assert.True(((ItemAddedToOrder)events.First()).Price == 15.00M);
        }

        [Test]
        public void RemoveItem()
        {
            var order = new Order();
            order.Hydrate(new OrderCreated("3"));
            order.Hydrate(new ItemAddedToOrder {OrderId = "3", Name = "Un Item"});

            var events = order.Execute(new RemoveItemFromOrder {OrderId = "3", Name = "Un Item"});
            
            Assert.True(events.Count() == 1);
            Assert.True(events.First() is ItemRemovedFromOrder);
        }

        [Test]
        public void MappingSubmitOrderFromV1ToV2()
        {
            var order = new Order();
            order.Hydrate(new OrderSubmitted());
        }
    }
}