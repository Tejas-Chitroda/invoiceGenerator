using Invoice_Generator.Data;
using Invoice_Generator.Models;
using Invoice_Generator.Repository;
using System;

namespace Invoice_Generator.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly InvoiceDbContext _context;
        public IGenericRepository<Category> Categories { get; }
        public IGenericRepository<Product> Products { get; }
        public IGenericRepository<ProductPrice> ProductPrices { get; }
        public IGenericRepository<Customer> Customers { get; }
        public IGenericRepository<Invoice> Invoices { get; }
        public IGenericRepository<InvoiceDetail> InvoiceDetails { get; }

        public UnitOfWork(InvoiceDbContext context)
        {
            _context = context;
            Categories = new GenericRepository<Category>(_context);
            Products = new GenericRepository<Product>(_context);
            ProductPrices = new GenericRepository<ProductPrice>(_context);
            Customers = new GenericRepository<Customer>(_context);
            Invoices = new GenericRepository<Invoice>(_context);
            InvoiceDetails = new GenericRepository<InvoiceDetail>(_context);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
