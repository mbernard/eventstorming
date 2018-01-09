using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class OutstandingOrders
    {
        public readonly IDictionary<string, (List<string> Items, OrderStatus Status)> _orders =
            new Dictionary<string, (List<string>, OrderStatus)>();

        public IEnumerable<KeyValuePair<string, (List<string> Items, OrderStatus Status)>> Orders =>
            _orders.Where(x => x.Value.Status == OrderStatus.Submitted);

        public void Apply(ItemAddedToOrder itemAddedToOrder)
        {
            if (_orders.ContainsKey(itemAddedToOrder.OrderId))
            {
                var addedOrder = _orders[itemAddedToOrder.OrderId];
                addedOrder.Items.Add(itemAddedToOrder.Name);
            }
            else
            {
                _orders.Add(itemAddedToOrder.OrderId, (new List<string> {itemAddedToOrder.Name}, OrderStatus.None));
            }
        }

        public void Apply(OrderSubmitted orderSubmitted)
        {
            var order = _orders[orderSubmitted.OrderId];
            _orders[orderSubmitted.OrderId] = (order.Items, OrderStatus.Submitted);
        }

        public void Apply(OrderPrepared orderPrepared)
        {
            _orders.Remove(orderPrepared.OrderId);
        }

        public void Apply(OrderCanceled orderCanceled)
        {
            _orders.Remove(orderCanceled.OrderId);
        }

        public void Apply(OrderStarted orderStarted)
        {
            var order = _orders[orderStarted.OrderId];
            _orders[orderStarted.OrderId] = (order.Items, OrderStatus.Started);
        }
    }
}
