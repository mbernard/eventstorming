using System;
using System.Collections.Generic;

using Infra;

namespace Domain
{
    public class Order : IAggregate
    {
        private string orderId;
        private bool received;
        private bool pickedUp;
        private OrderStatus Status;

        public IEnumerable<object> Execute(object order)
        {
            if (order is CreateOrder)
                return this.CreateOrder((CreateOrder)order);
            if (order is SubmitOrder)
                return this.SubmitOrder((SubmitOrder)order);
            if (order is CancelOrder)
            {
                return CancelOrder((CancelOrder) order);
            }
            if (order is FinishOrder)
            {
                //return FinishOrder((FinishOrder) order);
            }
            if (order is StartFoodPreparation)
            {
                return StartFoodPreparation((StartFoodPreparation) order);
            }
            throw new InvalidOperationException("Unknown command.");
        }

        private IEnumerable<object> CreateOrder(CreateOrder createOrder)
        {
            return new[] { new OrderCreated(createOrder.OrderId) };
        }

        private IEnumerable<object> StartFoodPreparation(StartFoodPreparation order)
        {
            return new[] {new OrderStarted()};
        }

        private IEnumerable<object> CancelOrder(CancelOrder cancelOrder)
        {
            switch (Status)
            {
                case OrderStatus.Submitted:
                case OrderStatus.Started:
                case OrderStatus.Prepared:
                    return new[] { new OrderCanceled() };
                case OrderStatus.PickedUp:
                    throw new Exception("Cannot cancel pickedup order");
                case OrderStatus.Delivered:
                    throw new Exception("Cannot cancel delivered order");
                default:
                    throw new OrderNotSubmittedException("Order not submitted.");
            }
        }

        private IEnumerable<object> SubmitOrder(SubmitOrder submitOrder)
        {
            return new[]
            {
                new OrderSubmitted()
            };
        }

        public string Id { get; set; }

        public void Hydrate(object @event)
        {
            if (@event is OrderCreated)
                this.OnOrderCreated((OrderCreated)@event);
            if (@event is OrderSubmitted)
                OnOrderSubmitted((OrderSubmitted) @event);
            if (@event is OrderPickedUp)
                OnOrderPickedUp((OrderPickedUp) @event);
        }

        private void OnOrderCreated(OrderCreated @event)
        {
            orderId = @event.OrderId;
        }

        private void OnOrderPickedUp(OrderPickedUp @event)
        {
            this.Status = OrderStatus.PickedUp;
        }

        private void OnOrderSubmitted(OrderSubmitted @event)
        {
            this.Status = OrderStatus.Submitted;
        }
    }

    public class SubmitOrder
    {
    }

    public enum OrderStatus
    {
        Submitted,
        Started,
        Prepared,
        PickedUp,
        Delivered
    }

    public class GetOrderStatus
    {

        public OrderStatus Status { get; set; }

        public void Apply(OrderSubmitted orderSubmitted)
        {
            Status = OrderStatus.Submitted;
        }

        public void Apply(OrderStarted orderStarted)
        {
            Status = OrderStatus.Started;
        }

        public void Apply(OrderPrepared orderPrepared)
        {
            Status = OrderStatus.Prepared;
        }

        public void Apply(OrderPickedUp orderPickedUp)
        {
            Status = OrderStatus.PickedUp;
        }

        public void Apply(FoodDelivered foodDelivered)
        {
            Status = OrderStatus.Delivered;
        }
    }

    public class OrderStarted
    {
    }

    public class OrderCreated
    {
        public string OrderId { get; }

        public OrderCreated(string orderId)
        {
            this.OrderId = orderId;
        }
    }

    public class FoodDelivered
    {
        
    }

    public class OrderPrepared
    {
    }

    public class OrderPerUser
    {
        public DateTime OrderDate { get; set; }

        public decimal TotalPrice { get; set; }

        public IList<(string Name, decimal Price)> Items { get; set; } = new List<(string, decimal)>();


        public void Apply(ItemAddedToOrder itemAddedToOrder)
        {
            Items.Add((itemAddedToOrder.Name, itemAddedToOrder.Price));
            TotalPrice += itemAddedToOrder.Price;
        }

        public void Apply(OrderSubmitted orderSubmitted)
        {
            OrderDate = orderSubmitted.Date;
        }
    }

    public class OrderCanceled
    {
    }
    
    public class FinishOrder
    {
    }

    public class OrderNotSubmittedException : Exception
    {
        public OrderNotSubmittedException(string message) : base(message)
        { }
    }

    public class CreateOrder
    {
        public string OrderId { get; }

        public CreateOrder(string orderId)
        {
            this.OrderId = orderId;
        }
    }

    public class CancelOrder
    {
        public CancelOrder(string orderId)
        {
            OrderId = orderId;
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

    public class StartFoodPreparation
    {
    }
}