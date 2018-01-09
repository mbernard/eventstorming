using System;
using System.Collections.Generic;
using System.Linq;
using Infra;

namespace Domain
{
    public class Order : IAggregate
    {
        public Order()
        {
        }

        private OrderStatus Status;
        private string OrderId;
        private bool isPaid;
        private List<string> items = new List<string>();
        
        public IEnumerable<object> Execute(object order)
        {
            if (order is CreateOrder)
                return this.CreateOrder((CreateOrder) order);
            if (order is CancelOrder)
                return CancelOrder((CancelOrder) order);
            if (order is FinishOrder)
                return FinishOrder((FinishOrder) order);
            if (order is StartFoodPreparation)
                return StartFoodPreparation((StartFoodPreparation) order);

            if (order is PickUpOrderForDelivery)
            {
                return PickUpOrder((PickUpOrderForDelivery) order);
            }

            if (order is ConfirmDelivery)
                return ConfirmDelivery((ConfirmDelivery) order);

            if (order is AddItemToOrder)
            {
                return AddItem((AddItemToOrder) order);
            }

            if (order is PayOrder)
            {
                return PayOrder((PayOrder) order);
            }

            if (order is RemoveItemFromOrder)
                return RemoveItemFromOrder((RemoveItemFromOrder) order);

            throw new InvalidOperationException("Unknown command.");
        }

        private IEnumerable<object> AddItem(AddItemToOrder order)
        {
            return new[]
            {
                new ItemAddedToOrder
                {
                    OrderId = this.OrderId,
                    Name = order.Name,
                    Price = order.Price
                }
            };
        }

        private IEnumerable<object> RemoveItemFromOrder(RemoveItemFromOrder removeItemFromOrder)
        {
            if (!items.Contains(removeItemFromOrder.Name))
                throw new Exception("Item doesn't exists in this order.");
            
            return new[]
            {
                new ItemRemovedFromOrder {OrderId = this.OrderId, Name = removeItemFromOrder.Name}
            };
        }

        private IEnumerable<object> PayOrder(PayOrder payOrder)
        {
            return new object[]
            {
                new OrderPaid(payOrder.OrderId, payOrder.Amount),
                new OrderSubmitted_V2 { OrderId = payOrder.OrderId, Date = DateTime.Now},
            };
        }

        private IEnumerable<object> CreateOrder(CreateOrder createOrder)
        {
            return new[] {new OrderCreated(createOrder.OrderId)};
        }

        private IEnumerable<object> PickUpOrder(PickUpOrderForDelivery order)
        {
            return new[] {new OrderPickedUp()};
        }

        private IEnumerable<object> StartFoodPreparation(StartFoodPreparation order)
        {
            return new[] {new OrderStarted()};
        }

        private IEnumerable<object> FinishOrder(FinishOrder finishOrder)
        {
            if (Status == OrderStatus.Started)
                return new[] {new OrderPrepared()};

            throw new InvalidOperationException("Cannot finish an unstarted order");
        }


        private IEnumerable<object> CancelOrder(CancelOrder cancelOrder)
        {
            switch (Status)
            {
                case OrderStatus.None:
                    throw new OrderNotSubmittedException("Order not submitted.");
                case OrderStatus.Submitted:
                case OrderStatus.Started:
                case OrderStatus.Prepared:
                    return new[] {new OrderCanceled()};
                case OrderStatus.PickedUp:
                    throw new Exception("Cannot cancel pickedup order");
                case OrderStatus.Delivered:
                    throw new Exception("Cannot cancel delivered order");
                default:
                    throw new OrderNotSubmittedException("Order not submitted.");
            }
        }

        public bool IsPaid { get; set; }

        private IEnumerable<object> ConfirmDelivery(ConfirmDelivery confirmDelivery)
        {
            return new[]
            {
                new OrderDelivered()
            };
        }

        public string Id { get; set; }

        public void Hydrate(object @event)
        {
            if (@event is OrderCreated)
                this.OnOrderCreated((OrderCreated) @event);
            if(@event is OrderSubmitted_V2)
                OnOrderSubmitted_V2((OrderSubmitted_V2) @event);
         if (@event is OrderStarted)
                OnOrderStarted((OrderStarted) @event);
            if (@event is OrderPickedUp)
                OnOrderPickedUp((OrderPickedUp) @event);
            if (@event is OrderPaid)
                OnOrderPaid((OrderPaid) @event);
            if (@event is ItemAddedToOrder)
                OnItemAddedToOrder((ItemAddedToOrder)@event);
        }

        private void OnItemAddedToOrder(ItemAddedToOrder @event)
        {
            this.items.Add(@event.Name);
        }

        private void OnOrderSubmitted_V2(OrderSubmitted_V2 @event)
        {
            Status = OrderStatus.Submitted;
        }

        private void OnOrderPaid(OrderPaid @event)
        {
            this.isPaid = true;
        }

        private void OnOrderCreated(OrderCreated @event)
        {
            this.OrderId = @event.OrderId;
        }

        private void OnOrderStarted(OrderStarted @event)
        {
            Status = OrderStatus.Started;
        }

        private void OnOrderPickedUp(OrderPickedUp @event)
        {
            Status = OrderStatus.PickedUp;
        }
    }

    public class SubmitOrder
    {
        public bool IsPaid { get; set; }
    }

    public enum OrderStatus
    {
        None = 0,
        Submitted = 1,
        Started = 2,
        Prepared = 3,
        ReadyForPickup = Prepared,
        PickedUp = 4,
        InTransit = PickedUp,
        Delivered = 5
    }

    public class GetOrderStatus
    {

        public OrderStatus Status { get; set; }

        public void Apply(OrderSubmitted orderSubmitted)
        {
            Status = OrderStatus.Submitted;
        }

        public void Apply(OrderSubmitted_V2 orderSubmitted)
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

        public void Apply(OrderDelivered orderDelivered)
        {
            Status = OrderStatus.Delivered;
        }
    }

    public class OrderStarted
    {
        public string OrderId { get; set; }
    }

    public class OrderCreated
    {
        public string OrderId { get; }

        public OrderCreated(string orderId)
        {
            this.OrderId = orderId;
        }
    }

    public class OrderDelivered
    {
        public string OrderId { get; set; }
    }

    public class OrderPrepared
    {
        public string OrderId { get; set; }
    }

    public class OrderCanceled
    {
        public string OrderId { get; set; }
    }

    public class FinishOrder
    {
    }

    public class OrderNotSubmittedException : Exception
    {
        public OrderNotSubmittedException(string message) : base(message)
        {
        }
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
        public string OrderId { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; }
    }

    public class OrderSubmitted_V2
    {
        public string OrderId { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; }
    }

    public class ItemAddedToOrder
    {
        public string OrderId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderPickedUp
    {
        public string OrderId { get; set; }
    }

    public class StartFoodPreparation
    {
    }

    public class PickUpOrderForDelivery
    {
    }

    public class OrderPaid
    {
        public OrderPaid(string orderId, decimal amount)
        {
            OrderId = orderId;
            Amount = amount;
        }

        public string OrderId { get; set; }
        public decimal Amount { get; set; }
    }

    public class ConfirmDelivery
    {
    }

    public class PayOrder
    {
        public PayOrder(string orderId, decimal amount)
        {
            this.OrderId = orderId;
            this.Amount = amount;
        }

        public string OrderId { get; }

        public decimal Amount { get; }
    }

    public class AddItemToOrder
    {
        public string Name { get; }
        public decimal Price { get; }

        public AddItemToOrder(string name, decimal price)
        {
            Name = name;
            Price = price;
        }
    }

    public class RemoveItemFromOrder
    {
        public string Name { get; set; }
    }

    public class ItemRemovedFromOrder
    {
        public string OrderId { get; set; }
        public string Name { get; set; }
    }
}