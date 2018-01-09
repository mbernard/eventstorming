using System.Collections.Generic;

namespace Domain
{
    public class DeliveryList
    {
        private IDictionary<string, (string Address, OrderStatus Status)> _deliveries = new Dictionary<string, (string, OrderStatus)>();
        
        public void Apply(OrderSubmitted_V2 orderSubmitted)
        {
            _deliveries.Add(orderSubmitted.OrderId, (orderSubmitted.Address, OrderStatus.Submitted));
        }

        public void Apply(OrderPrepared orderPrepared)
        {
            _deliveries[orderPrepared.OrderId] = (_deliveries[orderPrepared.OrderId].Address, OrderStatus.ReadyForPickup);
        }

        public IDictionary<string, (string Address, OrderStatus Status)> Deliveries => _deliveries;

        public void Apply(OrderCanceled orderCanceled)
        {
            _deliveries.Remove(orderCanceled.OrderId);
        }

        public void Apply(OrderPickedUp orderPickedUp)
        {
            _deliveries[orderPickedUp.OrderId] = (_deliveries[orderPickedUp.OrderId].Address, OrderStatus.PickedUp);
        }

        public void Apply(OrderDelivered orderDelivered)
        {
            _deliveries.Remove(orderDelivered.OrderId);
        }
    }
}