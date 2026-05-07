using Invoice_Generator.DTOs;

namespace Invoice_Generator.Services.Interfaces
{
    public interface ICustomerValidationService
    {
        Task<ValidationResultDto> ValidateCustomerAsync(CustomerDto customer);
    }
}
