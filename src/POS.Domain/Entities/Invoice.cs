using System.Collections.Generic;
using System.Linq;

namespace POS.Domain.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
        public decimal Total => Items.Sum(i => i.LineTotal);

        public Invoice() { }

        public Invoice(int customerId)
        {
            CustomerId = customerId;
        }

        public void AddItem(InvoiceItem item)
        {
            Items.Add(item);
        }
    }
}
