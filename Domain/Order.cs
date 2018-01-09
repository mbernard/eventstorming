using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain
{
    public class Order
    {
        private bool received;

        public IEnumerable<object> Execute(object order)
        {
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

            return new[] {new OrderCanceled()};
        }

        public void Hydrate(object @event)
        {
            if (@event is OrderReceived)
                this.OnOrderReceived((OrderReceived) @event);
        }

        private void OnOrderReceived(OrderReceived @event)
        {
            this.received = true;
        }
    }

    public enum OrderStatus
    {
        Submitted,
        Received,
        Started,
        Prepared,
        InTransit,
        Delivered
    }

    public class OrderPickedUp
    {
        
    }

    public class GetOrderStatus
    {
        public void Apply(OrderSubmitted orderSubmitted)
        {
            this.Status = OrderStatus.Submitted;
        }

        public OrderStatus Status { get; set; }

        public void Apply(OrderReceived orderReceived)
        {
            this.Status = OrderStatus.Received;
        }

        public void Apply(OrderStarted orderStarted)
        {
            this.Status = OrderStatus.Started;
        }

        public void Apply(OrederPrepared orederPrepared)
        {
            this.Status = OrderStatus.Prepared;
        }

        public void Apply(OrderPickedUp orderPickedUp)
        {
            this.Status = OrderStatus.InTransit;
        }

        public void Apply(FoodDelivered foodDelivered)
        {
            this.Status = OrderStatus.Delivered;

        }
    }

    public class FoodDelivered
    {
        
    }

    public class OrederPrepared
    {
    }

    public class OrdersPerUser
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
    }

    public class OrderStarted
    {
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
}