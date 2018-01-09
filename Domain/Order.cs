using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Infra;

namespace Domain
{
    public class Order : IAggregate
    {
        private bool received;
        private bool pickedUp;

        public IEnumerable<object> Execute(object order)
        {
            if (order is SubmitOrder)
                return this.ReceiveOrder((SubmitOrder)order);
            if (order is CancelOrder)
                return this.CancelOrder((CancelOrder) order);
            throw new InvalidOperationException("Unknown command.");
        }

        private IEnumerable<object> CancelOrder(CancelOrder cancelOrder)
        {
            if (!this.received)
            {
                throw new OrderNotReceivedException("Order not received.");
            }

            if (pickedUp)
            {
                throw new Exception("Cannot cancel pickedup order");
            }

            return new[] {new OrderCanceled()};
        }

        private IEnumerable<object> ReceiveOrder(SubmitOrder submitOrder)
        {
            return new[]
            {
                new OrderReceived(Guid.NewGuid().ToString())
            };
        }

        public string Id { get; set; }

        public void Hydrate(object @event)
        {
            if (@event is OrderReceived)
                this.OnOrderReceived((OrderReceived) @event);
            if (@event is OrderPickedUp)
                this.OnOrderPickedUp((OrderPickedUp)@event);
        }

        private void OnOrderPickedUp(OrderPickedUp @event)
        {
            pickedUp = true;
        }

        private void OnOrderReceived(OrderReceived @event)
        {
            this.received = true;
        }
    }

    public class SubmitOrder
    {
        public string OrderId { get; }

        public SubmitOrder(string orderId)
        {
            this.OrderId = orderId;
        }
    }

    public class OrderPerUser
    {
        public DateTime OrderDate { get; set; }

        public decimal TotalPrice { get; set; }

        public IList<(string Name, decimal Price)> Items { get; set; } = new List<(string, decimal)>();


        public void Apply(ItemAddedToOrder itemAddedToOrder)
        {
            this.Items.Add((itemAddedToOrder.Name, itemAddedToOrder.Price));
            this.TotalPrice += itemAddedToOrder.Price;
        }

        public void Apply(OrderSubmitted orderSubmitted)
        {
            this.OrderDate = orderSubmitted.Date;
        }
    }

    public class OrderCanceled
    {
    }

    public class OrderReceived
    {
        public string OrderId { get; }

        public OrderReceived(string orderId)
        {
            this.OrderId = orderId;
        }
    }

    public class OrderNotReceivedException : Exception
    {
        public OrderNotReceivedException(string message) : base(message)
        { }
    }

    public class CancelOrder
    {
        public CancelOrder(string orderId)
        {
            this.OrderId = orderId;
        }

        public string OrderId { get; }
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

    public class OrderPickedUp
    {
    }
}