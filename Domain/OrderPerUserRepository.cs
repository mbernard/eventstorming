using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain
{
    public class OrderPerUserRepository
    {
        private readonly Dictionary<string, OrderPerUser> _orderPerUsers = new Dictionary<string, OrderPerUser>();

        public void Insert(OrderPerUser orderPerUser)
        {
            this._orderPerUsers.Add(orderPerUser.OrderId, orderPerUser);
        }

        public OrderPerUser GetForOrder(string orderId)
        {
            return this._orderPerUsers[orderId];
        }
    }
}