using System.Threading.Tasks;
using POS.Domain.Entities;

namespace POS.Domain.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice> AddAsync(Invoice invoice);
    }
}
