using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infra;

namespace Domain
{
    public class OutstandingOrders
    {
        public readonly IDictionary<string, (List<string> Items, OrderStatus Status)> _orders = new Dictionary<string, (List<string>, OrderStatus)>();

        public void Apply(ItemAddedToOrder itemAddedToOrder)
        {
            if (this._orders.ContainsKey(itemAddedToOrder.OrderId))
            {
                var addedOrder = this._orders[itemAddedToOrder.OrderId];
                addedOrder.Items.Add(itemAddedToOrder.Name);
            }
            else
            {
                this._orders.Add(itemAddedToOrder.OrderId, (new List<string> { itemAddedToOrder.Name }, OrderStatus.None));
            }
        }

        public IEnumerable<KeyValuePair<string, (List<string> Items, OrderStatus Status)>> Orders =>
            this._orders.Where(x => x.Value.Status == OrderStatus.Submitted);

        public void Apply(OrderSubmitted orderSubmitted)
        {
            var order = this._orders[orderSubmitted.OrderId];
            this._orders[orderSubmitted.OrderId] = (order.Items, OrderStatus.Submitted);
        }

        public void Apply(OrderPrepared orderPrepared)
        {
            this._orders.Remove(orderPrepared.OrderId);
        }

        public void Apply(OrderCanceled orderCanceled)
        {
            this._orders.Remove(orderCanceled.OrderId);
        }
    }
}
