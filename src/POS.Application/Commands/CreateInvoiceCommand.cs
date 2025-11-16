using System.Collections.Generic;

namespace POS.Application.Commands
{
    public class InvoiceItemDto
    {
        public int ProductId { get; set; }
        public int Qty { get; set; }
    }

    public class CreateInvoiceCommand
    {
        public int CustomerId { get; set; }
        public List<InvoiceItemDto> Items { get; set; } = new List<InvoiceItemDto>();
    }
}
