using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZavrsniProjekat.Models;

namespace ZavrsniProjekat.Infrastructure
{
    public class ShoppingCart
     {
        private List<ShoppingCarItem> items = new List<ShoppingCarItem>();
        
        public void AddItem(Furniture furniture, int quantity)
        {
            ShoppingCarItem item = items
            .Where(p => p.Furniture.FurnitureId == furniture.FurnitureId)
            .FirstOrDefault();
            if (item == null)
            {
                items.Add(new ShoppingCarItem
                {
                    Furniture = furniture,
                    Quantity = quantity
                });
            }
            else
            {
                item.Quantity += quantity;
            }
        }

        public void RemoveItem(Furniture furniture)
        {
            items.RemoveAll(l => l.Furniture.FurnitureId == furniture.FurnitureId);
        }

        public void DecreaseQuantity(Furniture furniture)
        {
            ShoppingCarItem item = items.Where(p => p.Furniture.FurnitureId == furniture.FurnitureId).FirstOrDefault();
            if(item != null)
            {
                item.Quantity -= 1;
            }
            if(item.Quantity == 0)
            {
                RemoveItem(furniture);
            }
        }

        public decimal ComputeTotalValue()
        {
            return items.Sum(e => e.Furniture.Price * e.Quantity);
        }

        public decimal ComputeTotalValueWithTax()
        {
            decimal total = ComputeTotalValue();
            return total + total * Tax.RegularTax;
        }

        public void Clear()
        {
            items.Clear();
        }

        public IEnumerable<ShoppingCarItem> Items
        {             
            get { return items; }
        }

        
    }

    public class ShoppingCarItem
    {
        public Furniture Furniture { get; set; }
        public int Quantity { get; set; }        
    }    
}