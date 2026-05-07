using System.Net.Mail;
using Invoice_Generator.DTOs;
using Invoice_Generator.Services.Interfaces;
using Invoice_Generator.UoW;

namespace Invoice_Generator.Services.Implementations
{
    public class CustomerValidationService : ICustomerValidationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerValidationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ValidationResultDto> ValidateCustomerAsync(CustomerDto customer)
        {
            var result = new ValidationResultDto { IsValid = true };

            // Validate required fields
            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                result.IsValid = false;
                result.Errors.Add("Name", "Name is required");
            }

            if (string.IsNullOrWhiteSpace(customer.Email))
            {
                result.IsValid = false;
                result.Errors.Add("Email", "Email is required");
            }
            else
            {
                // Validate email format using .NET's MailAddress
                if (!IsValidEmail(customer.Email))
                {
                    result.IsValid = false;
                    result.Errors.Add("Email", "Invalid email format");
                }
                else
                {
                    // Check for duplicate email (case-insensitive)
                    var existingCustomer = await _unitOfWork.Customers
                        .FindAsync(c => c.Email != null && c.Email.ToLower() == customer.Email.ToLower());
                    
                    if (existingCustomer.Any())
                    {
                        result.IsValid = false;
                        result.Errors.Add("Email", "Email already exists");
                    }
                }
            }

            return result;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
