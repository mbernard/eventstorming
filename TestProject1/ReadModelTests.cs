using System;
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
            
            outstandingOrders.Apply(new ItemAddedToOrder {OrderId = "3", Name = "Pizza", Price = 2});
            outstandingOrders.Apply(new ItemAddedToOrder {OrderId = "3", Name = "Burger", Price = 1});
            outstandingOrders.Apply(new OrderSubmitted {OrderId = "3", Date = DateTime.Now});
            outstandingOrders.Apply(new ItemAddedToOrder {OrderId = "5", Name = "Chicken Wings", Price = 1});
            outstandingOrders.Apply(new ItemAddedToOrder {OrderId = "5", Name = "Poutine", Price = 2});
            outstandingOrders.Apply(new OrderSubmitted {OrderId = "5", Date = DateTime.Now});
            
            //When
            outstandingOrders.Apply(new OrderCanceled {OrderId = "3"});
            
            //Then
            Assert.True(outstandingOrders.Orders.Count() == 1);
            var firstOrder = outstandingOrders.Orders.First();
            Assert.True(firstOrder.Key == "5");
            Assert.True(firstOrder.Value.Status != OrderStatus.Started);
            var firstOrderItems = firstOrder.Value.Items;
            Assert.True(firstOrderItems.First() == "Chicken Wings");
            Assert.True(firstOrderItems.ElementAt(1) == "Poutine");
        }
		
		[Test]
        public void StartOrderChangeOustandingOrderToStarted()
        {
            // Given
            var outstandingOrders = new OutstandingOrders();
            outstandingOrders.Apply(new ItemAddedToOrder { OrderId = "3", Name = "Hamburger", Price = 5 });
            outstandingOrders.Apply(new ItemAddedToOrder { OrderId = "3", Name = "Pizza", Price = 5 });
            outstandingOrders.Apply(new OrderSubmitted { OrderId = "3" });
            outstandingOrders.Apply(new ItemAddedToOrder { OrderId = "5", Name = "Chicken Wings", Price = 10 });
            outstandingOrders.Apply(new OrderSubmitted { OrderId = "5" });
            outstandingOrders.Apply(new OrderStarted() { OrderId = "5" });

            // When
            // Then
            Assert.AreEqual(OrderStatus.Started, outstandingOrders._orders["5"].Status);
        }

        [Test]
        public void DeliveryList_When_OrderPrepared()
        {
            //Given
            var deliveryList = new DeliveryList();
            deliveryList.Apply(new OrderSubmitted {OrderId = "3", Date = DateTime.Now, Address = "1234 rue Louis, Mtl"});
            deliveryList.Apply(new OrderPrepared {OrderId = "3"});
            deliveryList.Apply(new OrderSubmitted {OrderId = "5", Date = DateTime.Now, Address = "5678 rue Justin, Mtl"});
            
            //When
            deliveryList.Apply(new OrderPrepared {OrderId = "5"});

            //Then
            Assert.True(deliveryList.Deliveries.Count() == 2);
            Assert.True(deliveryList.Deliveries.First().Key == "3");
            Assert.True(deliveryList.Deliveries.First().Value.Address == "1234 rue Louis, Mtl");
            Assert.True(deliveryList.Deliveries.First().Value.Status == OrderStatus.ReadyForPickup);
            Assert.True(deliveryList.Deliveries.ElementAt(1).Key == "5");
            Assert.True(deliveryList.Deliveries.ElementAt(1).Value.Address == "5678 rue Justin, Mtl");
            Assert.True(deliveryList.Deliveries.ElementAt(1).Value.Status == OrderStatus.ReadyForPickup);
        }

        [Test]
        public void DeliveryList_When_OrderPickedUp()
        {
            //Given
            var deliveryList = new DeliveryList();
            deliveryList.Apply(new OrderSubmitted {OrderId = "3", Date = DateTime.Now, Address = "1234 rue Louis, Mtl"});
            deliveryList.Apply(new OrderPrepared {OrderId = "3"});
            deliveryList.Apply(new OrderSubmitted {OrderId = "5", Date = DateTime.Now, Address = "5678 rue Justin, Mtl"});
            deliveryList.Apply(new OrderPrepared {OrderId = "5"});
            deliveryList.Apply(new OrderCanceled {OrderId = "3"});
            deliveryList.Apply(new OrderPickedUp {OrderId = "5"});
            
            //When
            //Then
            Assert.True(deliveryList.Deliveries.Count() == 1);
            Assert.True(deliveryList.Deliveries.First().Key == "5");
            Assert.True(deliveryList.Deliveries.First().Value.Address == "5678 rue Justin, Mtl");
            Assert.True(deliveryList.Deliveries.First().Value.Status == OrderStatus.PickedUp);
        }
        
        [Test]
        public void DeliveryList_When_OrderDelivered()
        {
            //Given
            var deliveryList = new DeliveryList();
            deliveryList.Apply(new OrderSubmitted {OrderId = "3", Date = DateTime.Now, Address = "1234 rue Louis, Mtl"});
            deliveryList.Apply(new OrderPrepared {OrderId = "3"});
            deliveryList.Apply(new OrderSubmitted {OrderId = "5", Date = DateTime.Now, Address = "5678 rue Justin, Mtl"});
            deliveryList.Apply(new OrderPrepared {OrderId = "5"});
            deliveryList.Apply(new OrderPickedUp {OrderId = "3"});
            deliveryList.Apply(new OrderPickedUp {OrderId = "5"});
            deliveryList.Apply(new OrderDelivered {OrderId = "3"});
            
            //When
            //Then
            Assert.True(deliveryList.Deliveries.Count() == 1);
            Assert.True(deliveryList.Deliveries.First().Key == "5");
            Assert.True(deliveryList.Deliveries.First().Value.Address == "5678 rue Justin, Mtl");
            Assert.True(deliveryList.Deliveries.First().Value.Status == OrderStatus.InTransit);
        }
        
    }
}