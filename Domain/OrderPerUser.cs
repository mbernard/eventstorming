using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain
{
    public class OrderPerUser
    {
        public string OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalPrice { get; set; }

        public IList<(string Name, decimal Price)> Items { get; set; } = new List<(string, decimal)>();
        
        public void Apply(ItemAddedToOrder itemAddedToOrder)
        {
            this.Items.Add((itemAddedToOrder.Name, itemAddedToOrder.Price));
            this.TotalPrice += itemAddedToOrder.Price;
        }

        public void Apply(ItemRemovedFromOrder itemRemovedFromOrder)
        {
            var newList = new List<(string Name, decimal Price)>();

            bool found = false;
            
            foreach (var item in this.Items)
            {
                if (item.Name != itemRemovedFromOrder.Name || found)
                {
                    newList.Add(item);
                }
                else
                {
                    found = true;
                }
            }

            Items = newList;
            this.TotalPrice = this.Items.Sum(x => x.Price);
        }
        
        public void Apply(OrderSubmitted orderSubmitted)
        {
            this.OrderDate = orderSubmitted.Date;
        }
    }
}