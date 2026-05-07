using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Invoice_Generator.UoW;

namespace Invoice_Generator.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveAsync();
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null) return false;

            _unitOfWork.Customers.Delete(customer);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _unitOfWork.Customers.GetAllAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _unitOfWork.Customers.GetByIdAsync(id);
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            var existing = await _unitOfWork.Customers.GetByIdAsync(customer.Id);
            if (existing == null) return false;

            existing.Name = customer.Name;
            existing.Email = customer.Email;

            _unitOfWork.Customers.Update(existing);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
