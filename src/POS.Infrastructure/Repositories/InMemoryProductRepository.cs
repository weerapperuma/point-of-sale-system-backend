using System.Collections.Concurrent;
using System.Threading.Tasks;
using POS.Domain.Entities;
using POS.Domain.Interfaces;

namespace POS.Infrastructure.Repositories
{
    public class InMemoryProductRepository : IProductRepository
    {
        private readonly ConcurrentDictionary<int, Product> _store = new ConcurrentDictionary<int, Product>();

        public InMemoryProductRepository()
        {
            // Seed some products matching README scenario (IDs 1 and 5)
            _store[1] = new Product(1, "Widget A", 9.99m, 10);
            _store[5] = new Product(5, "Widget B", 4.50m, 5);
        }

        public Task<Product?> GetByIdAsync(int id)
        {
            _store.TryGetValue(id, out var p);
            return Task.FromResult<Product?>(p);
        }

        public Task UpdateAsync(Product product)
        {
            _store[product.Id] = product;
            return Task.CompletedTask;
        }
    }
}
