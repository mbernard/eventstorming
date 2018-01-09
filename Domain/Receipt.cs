using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class Receipt
    {
        public readonly IDictionary<string, (List<(string Name, decimal Price)> Items, decimal Price, string Status)> _receipts =
            new Dictionary<string, (List<(string, decimal)>, decimal, string)>();

        public IEnumerable<KeyValuePair<string, (List<(string Name, decimal Price)> Items, decimal Price, string Status)>> Receipts => _receipts;
        
        public void Apply(OrderCreated orderSubmitted)
        {
            _receipts.Add(orderSubmitted.OrderId, (new List<(string, decimal)>(), 0, "NotPaid"));
        }

        public void Apply(ItemAddedToOrder itemAddedToOrder)
        {
            var items = _receipts[itemAddedToOrder.OrderId].Items;
            items.Add((itemAddedToOrder.Name, itemAddedToOrder.Price));
            var price = _receipts[itemAddedToOrder.OrderId].Price + itemAddedToOrder.Price;
            _receipts[itemAddedToOrder.OrderId] = (items, price, "NotPaid");
        }

        public void Apply(ItemRemovedFromOrder itemRemovedFromOrder)
        {
            var items = _receipts[itemRemovedFromOrder.OrderId].Items;
            var item = items.First(x => x.Name == itemRemovedFromOrder.Name);
            items.Remove(item);
            var price = _receipts[itemRemovedFromOrder.OrderId].Price - item.Price;
            _receipts[itemRemovedFromOrder.OrderId] = (items, price, "NotPaid");
        }

        public void Apply(OrderPaidOnDelivery orderPaidOnDelivery)
        {
            var items = _receipts[orderPaidOnDelivery.OrderId].Items;
            var price = _receipts[orderPaidOnDelivery.OrderId].Price;
            _receipts[orderPaidOnDelivery.OrderId] = (items, price, "Paid");
        }
    }
}
