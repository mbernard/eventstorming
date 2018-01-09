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
                
                if (@event is  ItemRemovedFromOrder)
                {
                    var itemRemoved = (ItemRemovedFromOrder) @event;
                    var orderPerUser = orderPerUserRepository.GetForOrder(itemAdded.OrderId);
                    orderPerUser.Apply(itemRemoved);
                }
            };
        };
    }
}
