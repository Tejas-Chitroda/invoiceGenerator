using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Invoice_Generator.UoW;

namespace Invoice_Generator.Services.Implementations
{
    public class IProductPriceService : IProductPrice
    {
        private readonly IUnitOfWork _unitOfWork;

        public IProductPriceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task AddPriceAsync(ProductPrice productPrice)
        {
            await _unitOfWork.ProductPrices.AddAsync(productPrice);
            await _unitOfWork.SaveAsync();
        }

        public async Task AddPriceWithDefaultPriceAsync(ProductPriceDto productPriceDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productPriceDto.ProductId);

            if(product == null)
            {
                throw new Exception("Product Not Found");
            }

            if(productPriceDto.IsDefault)
            {
                var defaultPrice = await _unitOfWork.ProductPrices
                    .FindAsync(p => p.ProductId == productPriceDto.ProductId && p.IsDefault);

                if(defaultPrice.Any())
                {
                    throw new Exception("Default price already exists for this product.");
                }

                var newPrice = new ProductPrice
                {
                    ProductId = productPriceDto.ProductId,
                    Price = productPriceDto.Price,
                    EffectiveFrom = null,
                    EffectiveTo = null,
                    IsDefault = true
                };

                await _unitOfWork.ProductPrices.AddAsync(newPrice);
            }
            else
            {
                if(productPriceDto.EffectiveFrom == null || productPriceDto.EffectiveTo == null)
                {
                    throw new Exception("Effective dates must be provided for non-default prices.");
                }
                if(productPriceDto.EffectiveFrom >= productPriceDto.EffectiveTo)
                {
                    throw new Exception("Effective From date must be earlier than Effective To date.");
                }

                var existingPrice = await _unitOfWork.ProductPrices.FindAsync(p => p.ProductId == productPriceDto.ProductId && !p.IsDefault);

                if (existingPrice.Any(p => p.EffectiveFrom <= productPriceDto.EffectiveTo && p.EffectiveTo >= productPriceDto.EffectiveFrom))
                {
                    throw new Exception("There is already a price for this product that overlaps with the provided effective dates.");
                }

                var newPrice = new ProductPrice
                {
                    ProductId = productPriceDto.ProductId,
                    Price = productPriceDto.Price,
                    EffectiveFrom = productPriceDto.EffectiveFrom,
                    EffectiveTo = productPriceDto.EffectiveTo,
                    IsDefault = false
                };

                await _unitOfWork.ProductPrices.AddAsync(newPrice);
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task<bool> DeletePriceAsync(int id)
        {
            var price = await _unitOfWork.ProductPrices.GetByIdAsync(id);
            if(price == null)
            {
                return false;
            }
            _unitOfWork.ProductPrices.Delete(price);
            return true;
        }

        public Task<IEnumerable<ProductPrice>> GetAllPriceAsync()
        {
            return _unitOfWork.ProductPrices.GetAllAsync();
        }

        public Task<ProductPrice?> GetPriceByIdAsync(int id)
        {
            return _unitOfWork.ProductPrices.GetByIdAsync(id);
        }

        public Task<ProductPrice> GetPriceByProductIdAsync(int productId)
        {

            var today = DateTime.UtcNow;

            var price =  _unitOfWork.ProductPrices
                .Query()
                .Where(p => p.ProductId == productId &&
                            p.EffectiveFrom <= today &&
                            p.EffectiveTo >= today)
                .OrderByDescending(p => p.EffectiveFrom)
                .FirstOrDefault();

            return Task.FromResult(price);
        }

        public async Task<bool> UpdatePriceAsync(ProductPrice productPrice)
        {
            var existing = await _unitOfWork.ProductPrices.GetByIdAsync(productPrice.Id);

            existing.EffectiveFrom = productPrice.EffectiveFrom;
            existing.EffectiveTo = productPrice.EffectiveTo;
            existing.Price = productPrice.Price;
            existing.ProductId = productPrice.ProductId;

            _unitOfWork.ProductPrices.Update(existing);
            await _unitOfWork.SaveAsync();
            return true;

        }
    }
}
