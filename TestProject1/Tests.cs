using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TestProject1
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TotalPriceIsCorrect()
        {
            // Given
            // When
            var ordersPersUser = new OrdersPerUser();
            ordersPersUser.Apply(new ItemAddedToOrder() { Name = "Item1", Price = 12.34M});
            ordersPersUser.Apply(new ItemAddedToOrder() { Name = "Item2", Price = 34.56M});
            ordersPersUser.Apply(new OrderSubmitted() { Date = new DateTime(2010, 10, 10)});

            // Then
            Assert.AreEqual(46.90M, ordersPersUser.TotalPrice);
        }
    }

    public class OrdersPerUser
    {
        public DateTime OrderDate { get; set; }

        public decimal TotalPrice { get; set; }

        public IList<(string Name, decimal Price)> Items { get; set; } = new List<(string, decimal)>();

        public void Apply(ItemAddedToOrder itemAddedToOrder)
        {
            Items.Add((itemAddedToOrder.Name, itemAddedToOrder.Price));
            this.TotalPrice += itemAddedToOrder.Price;
        }

        public void Apply(OrderSubmitted orderSubmitted)
        {
            OrderDate = orderSubmitted.Date;
        }
    }

    public class OrderSubmitted
    {
        public DateTime Date { get; set; }
    }

    public class ItemAddedToOrder
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}