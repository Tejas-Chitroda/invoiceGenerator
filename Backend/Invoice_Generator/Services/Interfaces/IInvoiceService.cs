using Invoice_Generator.DTOs;
using Invoice_Generator.Models;

namespace Invoice_Generator.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        Task<Invoice?> GetInvoiceByIdAsync(int id);
        Task AddInvoiceAsync(InvoiceRequestDto invoice);
        Task<bool> DeleteInvoiceAsync(int id);
        Task<IEnumerable<InvoiceDetail>> GetInvoiceDetailByInvoiceId(int invoiceId);
    }
}
