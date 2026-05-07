using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromBody] CustomerDto customer)
        {
            if (customer == null)
            {
                return BadRequest("Customer cannot be null");
            }

            var customerModel = new Customer
            {
                Name = customer.Name,
                Email = customer.Email
            };

            await _customerService.AddCustomerAsync(customerModel);
            return Ok(customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer([FromBody] CustomerDto customer , int id)
        {
            if (customer == null)
            {
                return BadRequest("Customer cannot be null");
            }

            var customerModel = new Customer
            { 
                Id = id,
                Name = customer.Name,
                Email = customer.Email

            };
            var updated = await _customerService.UpdateCustomerAsync(customerModel);
            if (!updated)
            {
                return NotFound();
            }
            return Ok(customerModel);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var deleted = await _customerService.DeleteCustomerAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
