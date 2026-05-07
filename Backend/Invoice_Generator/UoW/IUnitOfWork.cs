using Invoice_Generator.Models;
using Invoice_Generator.Repository;

namespace Invoice_Generator.UoW
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Product> Products { get; }
        IGenericRepository<ProductPrice> ProductPrices { get; }
        IGenericRepository<Customer> Customers { get; }
        IGenericRepository<Invoice> Invoices { get; }
        IGenericRepository<InvoiceDetail> InvoiceDetails { get; }

        Task<int> SaveAsync();
    }
}
