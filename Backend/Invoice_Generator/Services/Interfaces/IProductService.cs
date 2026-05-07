using Invoice_Generator.Models;
using InvoiceGenerator.DTOs;

namespace Invoice_Generator.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductGetDto>> GetAllProductAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<decimal?> GetTodaysPriceAsync(int productId);
        Task AddProductAsync(Product product);
        Task<bool> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
    }
}
