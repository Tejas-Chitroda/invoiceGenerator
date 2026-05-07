using Invoice_Generator.DTOs;
using Invoice_Generator.Migrations;
using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Invoice_Generator.UoW;
using InvoiceGenerator.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Invoice_Generator.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<ProductGetDto>> GetAllProductAsync()
        {
            var products = await _unitOfWork.Products.Query().Include(p => p.Category).ThenInclude(p => p.Products).ToListAsync();

            return products.Select(p => new ProductGetDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                TaxPercentage = p.TaxPercentage,

                Category = p.Category == null ? null : new CategoryGetDto
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name,
                    Description = p.Category.Description
                },

                Prices = p.Prices?.Select(price => new ProductPriceGetDto
                {
                    Id = price.Id,
                    Price = price.Price,
                    EffectiveFrom = (DateTime)price.EffectiveFrom,
                    EffectiveTo = price.EffectiveTo
                }).ToList() ?? new List<ProductPriceGetDto>()
            });

        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _unitOfWork.Products.GetByIdAsync(id);
        }

        public async Task AddProductAsync(Product product)
        {
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveAsync();
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            var existing = await _unitOfWork.Products.GetByIdAsync(product.Id);
            if (existing == null) return false;

            existing.Name = product.Name;
            existing.TaxPercentage = product.TaxPercentage;
            existing.CategoryId = product.CategoryId;
            existing.Description = product.Description;

            _unitOfWork.Products.Update(existing);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _unitOfWork.Products.GetByIdAsync(id);
            if (existing == null) return false;

            _unitOfWork.Products.Delete(existing);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<decimal?> GetTodaysPriceAsync(int productId)
        {
            var now = DateTime.UtcNow;

            var defaultProductPrice = (await _unitOfWork.ProductPrices.FindAsync(p => p.ProductId == productId && p.IsDefault)).FirstOrDefault();

            var produtPriceForToday = await _unitOfWork.ProductPrices.FindAsync(p =>
                p.ProductId == productId &&
                !p.IsDefault &&
                p.EffectiveFrom.HasValue &&
                p.EffectiveTo.HasValue &&
                p.EffectiveFrom <= now &&
                p.EffectiveTo >= now);

            var price = produtPriceForToday.FirstOrDefault();

            if (price != null)
                return price.Price;

            return defaultProductPrice?.Price;
        }
    }
}
