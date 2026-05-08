using Invoice_Generator.DTOs;
using Invoice_Generator.Services.Interfaces;

namespace Invoice_Generator.Services.Implementations
{
    public class ProductValidationService : IProductValidationService
    {
        public Task<ValidationResultDto> ValidateProductAsync(ProductDto product)
        {
            var result = new ValidationResultDto { IsValid = true };

            if (product == null)
            {
                result.IsValid = false;
                result.Errors.Add("Product", "Product cannot be null");
                return Task.FromResult(result);
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                result.IsValid = false;
                result.Errors.Add("Name", "Name is required");
            }

            if (product.CategoryId <= 0)
            {
                result.IsValid = false;
                result.Errors.Add("CategoryId", "CategoryId must be a positive integer");
            }

            if (product.TaxPercentage < 0 || product.TaxPercentage > 100)
            {
                result.IsValid = false;
                result.Errors.Add("TaxPercentage", "TaxPercentage must be between 0 and 100");
            }

            return Task.FromResult(result);
        }
    }
}
