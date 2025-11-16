using System.Collections.Concurrent;
using System.Threading.Tasks;
using POS.Domain.Entities;
using POS.Domain.Interfaces;

namespace POS.Infrastructure.Repositories
{
    public class InMemoryInvoiceRepository : IInvoiceRepository
    {
        private readonly ConcurrentDictionary<int, Invoice> _store = new ConcurrentDictionary<int, Invoice>();
        private int _idCounter = 1000;

        public Task<Invoice> AddAsync(Invoice invoice)
        {
            var id = System.Threading.Interlocked.Increment(ref _idCounter);
            invoice.Id = id;
            _store[id] = invoice;
            return Task.FromResult(invoice);
        }
    }
}
