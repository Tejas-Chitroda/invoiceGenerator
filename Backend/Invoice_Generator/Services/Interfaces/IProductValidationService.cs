using Invoice_Generator.DTOs;

namespace Invoice_Generator.Services.Interfaces
{
    public interface IProductValidationService
    {
        Task<ValidationResultDto> ValidateProductAsync(ProductDto product);
    }
}
