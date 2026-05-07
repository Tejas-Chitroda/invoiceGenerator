using Invoice_Generator.DTOs;
using Invoice_Generator.Models;

namespace Invoice_Generator.Services.Interfaces
{
    public interface IProductPrice
    {
        Task<IEnumerable<ProductPrice>> GetAllPriceAsync();
        Task<ProductPrice?> GetPriceByIdAsync(int id);
        Task<ProductPrice> GetPriceByProductIdAsync(int productId);
        Task AddPriceAsync(ProductPrice productPrice);
        Task<bool> UpdatePriceAsync(ProductPrice productPrice);
        Task<bool> DeletePriceAsync(int id);

        Task AddPriceWithDefaultPriceAsync(ProductPriceDto productPriceDto);
    }
}
