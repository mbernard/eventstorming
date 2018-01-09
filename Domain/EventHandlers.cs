using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public static class EventHandlers
    {
        public static Func<OrderPerUserRepository, Action<object>> OnOrderCreatedEventHandler = (orderPerUserRepository) =>
        {
            return @event =>
            {
                if (@event is OrderCreated)
                {
                    orderPerUserRepository.Insert(
                        new OrderPerUser
                        {
                            OrderId = ((OrderCreated)@event).OrderId
                        });
                }

                if (@event is  ItemAddedToOrder)
                {
                    var itemAdded = (ItemAddedToOrder) @event;
                    var orderPerUser = orderPerUserRepository.GetForOrder(itemAdded.OrderId);
                    orderPerUser.Apply(itemAdded);
                }
            };
        };

        public static Func<OutstandingOrders, Action<object>> OutstandingOrdersEventHandlers =  outstandingOrders =>
        {
            return @event =>
            {
                if (@event is ItemAddedToOrder)
                {
                    outstandingOrders.Apply((ItemAddedToOrder)@event);
                }

                if (@event is OrderSubmitted)
                {
                    outstandingOrders.Apply((OrderSubmitted)@event);
                }

                if (@event is OrderCanceled)
                {
                    outstandingOrders.Apply((OrderCanceled)@event);
                }

                if (@event is OrderPrepared)
                {
                    outstandingOrders.Apply((OrderPrepared)@event);
                }

                if (@event is OrderStarted)
                {
                    outstandingOrders.Apply((OrderStarted)@event);
                }
            };
        };
    }
}
