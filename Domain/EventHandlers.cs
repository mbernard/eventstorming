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
            };
        };
    }
}
