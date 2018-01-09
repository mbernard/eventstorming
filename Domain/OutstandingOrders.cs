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
        private readonly IDictionary<string, List<string>> _addedOrders = new Dictionary<string, List<string>>();

        public readonly IDictionary<string, List<string>> Orders = new Dictionary<string, List<string>>();

        public void Apply(ItemAddedToOrder itemAddedToOrder)
        {
            if (this._addedOrders.ContainsKey(itemAddedToOrder.OrderId))
            {
                this._addedOrders[itemAddedToOrder.OrderId].Add(itemAddedToOrder.Name);
            }
            else
            {
                this._addedOrders.Add(itemAddedToOrder.OrderId, new List<string> { itemAddedToOrder.Name });
            }
        }

        public void Apply(OrderSubmitted orderSubmitted)
        {
            this.Orders.Add(orderSubmitted.OrderId, this._addedOrders[orderSubmitted.OrderId]);
            this._addedOrders.Remove(orderSubmitted.OrderId);
        }

        public void Apply(OrderPrepared orderPrepared)
        {
            this.Orders.Remove(orderPrepared.OrderId);
        }
    }
}
