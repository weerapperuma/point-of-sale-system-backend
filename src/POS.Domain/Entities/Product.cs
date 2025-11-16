using System;

namespace POS.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int QuantityAvailable { get; private set; }

        public Product() { }

        public Product(int id, string name, decimal price, int qty)
        {
            Id = id;
            Name = name;
            UnitPrice = price;
            QuantityAvailable = qty;
        }

        public void ReduceStock(int amount)
        {
            if (amount <= 0) throw new ArgumentException("Quantity must be positive", nameof(amount));
            if (amount > QuantityAvailable) throw new InvalidOperationException("Insufficient stock");
            QuantityAvailable -= amount;
        }
    }
}
