using System.Threading.Tasks;
using POS.Domain.Interfaces;

namespace POS.Infrastructure.UnitOfWork
{
    public class InMemoryUnitOfWork : IUnitOfWork
    {
        public Task SaveChangesAsync()
        {
            // In-memory: nothing to flush. In real EF Core UoW, call SaveChangesAsync on DbContext.
            return Task.CompletedTask;
        }
    }
}
