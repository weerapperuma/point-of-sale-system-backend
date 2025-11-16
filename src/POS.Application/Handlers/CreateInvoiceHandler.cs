using System;
using System.Linq;
using System.Threading.Tasks;
using POS.Application.Commands;
using POS.Domain.Entities;
using POS.Domain.Interfaces;

namespace POS.Application.Handlers
{
    public class CreateInvoiceHandler
    {
        private readonly IProductRepository _productRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateInvoiceHandler(IProductRepository productRepository, IInvoiceRepository invoiceRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _invoiceRepository = invoiceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> HandleAsync(CreateInvoiceCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (command.Items == null || !command.Items.Any()) throw new ArgumentException("Invoice must have at least one item");

            var invoice = new Invoice(command.CustomerId);

            // Load products and validate stock
            foreach (var item in command.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null) throw new InvalidOperationException($"Product {item.ProductId} not found");
                if (item.Qty <= 0) throw new InvalidOperationException("Quantity must be greater than zero");
                if (item.Qty > product.QuantityAvailable) throw new InvalidOperationException($"Insufficient stock for product {product.Id}");

                product.ReduceStock(item.Qty);

                var invoiceItem = new InvoiceItem
                {
                    ProductId = product.Id,
                    Quantity = item.Qty,
                    UnitPrice = product.UnitPrice
                };

                invoice.AddItem(invoiceItem);

                await _productRepository.UpdateAsync(product);
            }

            var created = await _invoiceRepository.AddAsync(invoice);
            await _unitOfWork.SaveChangesAsync();

            return created.Id;
        }
    }
}
