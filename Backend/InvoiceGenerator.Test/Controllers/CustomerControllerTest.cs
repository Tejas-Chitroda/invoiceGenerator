using System.Collections.Generic;
using System.Threading.Tasks;
using Invoice_Generator.Controllers;
using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InvoiceGenerator.Test.Controllers
{
    public class CustomerControllerTest
    {
        private readonly Mock<ICustomerService> _mockService;
        private readonly Mock<ICustomerValidationService> _mockValidationService;
        private readonly CustomerController _controller;

        public CustomerControllerTest()
        {
            _mockService = new Mock<ICustomerService>();
            _mockValidationService = new Mock<ICustomerValidationService>();
            _controller = new CustomerController(_mockService.Object, _mockValidationService.Object);
        }

        [Fact]
        public async Task GetAllCustomers_ReturnsOkResult_WithListOfCustomers()
        {
            var customers = new List<Customer> { new Customer { Id = 1, Name = "Test" } };
            _mockService.Setup(s => s.GetAllCustomersAsync()).ReturnsAsync(customers);

            var result = await _controller.GetAllCustomers();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(customers, okResult.Value);
        }

        [Fact]
        public async Task GetCustomerById_ReturnsOkResult_WhenCustomerExists()
        {
            var customer = new Customer { Id = 1, Name = "Test" };
            _mockService.Setup(s => s.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

            var result = await _controller.GetCustomerById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(customer, okResult.Value);
        }

        [Fact]
        public async Task GetCustomerById_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            _mockService.Setup(s => s.GetCustomerByIdAsync(1)).ReturnsAsync((Customer)null);

            var result = await _controller.GetCustomerById(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddCustomer_ReturnsOkResult_WhenCustomerIsValid()
        {
            var dto = new CustomerDto { Name = "Test", Email = "test@example.com" };
            var validationResult = new ValidationResultDto { IsValid = true };
            _mockValidationService.Setup(v => v.ValidateCustomerAsync(dto)).ReturnsAsync(validationResult);

            var result = await _controller.AddCustomer(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, okResult.Value);
            _mockService.Verify(s => s.AddCustomerAsync(It.IsAny<Customer>()), Times.Once);
        }

        [Fact]
        public async Task AddCustomer_ReturnsBadRequest_WhenCustomerIsNull()
        {
            var result = await _controller.AddCustomer(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Customer cannot be null", badRequest.Value);
        }

        [Fact]
        public async Task AddCustomer_ReturnsBadRequest_WhenValidationFails()
        {
            var dto = new CustomerDto { Name = "", Email = "invalid" };
            var validationResult = new ValidationResultDto
            {
                IsValid = false,
                Errors = new Dictionary<string, string>
                {
                    { "Name", "Name is required" },
                    { "Email", "Invalid email format" }
                }
            };
            _mockValidationService.Setup(v => v.ValidateCustomerAsync(dto)).ReturnsAsync(validationResult);

            var result = await _controller.AddCustomer(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var returnedResult = Assert.IsType<ValidationResultDto>(badRequest.Value);
            Assert.False(returnedResult.IsValid);
            Assert.Equal(2, returnedResult.Errors.Count);
            _mockService.Verify(s => s.AddCustomerAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task AddCustomer_ReturnsBadRequest_WhenEmailIsDuplicate()
        {
            var dto = new CustomerDto { Name = "Test", Email = "existing@example.com" };
            var validationResult = new ValidationResultDto
            {
                IsValid = false,
                Errors = new Dictionary<string, string>
                {
                    { "Email", "Email already exists" }
                }
            };
            _mockValidationService.Setup(v => v.ValidateCustomerAsync(dto)).ReturnsAsync(validationResult);

            var result = await _controller.AddCustomer(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var returnedResult = Assert.IsType<ValidationResultDto>(badRequest.Value);
            Assert.False(returnedResult.IsValid);
            Assert.True(returnedResult.Errors.ContainsKey("Email"));
            _mockService.Verify(s => s.AddCustomerAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCustomer_ReturnsOkResult_WhenUpdateSucceeds()
        {
            var dto = new CustomerDto { Name = "Test", Email = "test@example.com" };
            _mockService.Setup(s => s.UpdateCustomerAsync(It.IsAny<Customer>())).ReturnsAsync(true);

            var result = await _controller.UpdateCustomer(dto, 1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var customer = Assert.IsType<Customer>(okResult.Value);
            Assert.Equal(1, customer.Id);
            Assert.Equal(dto.Name, customer.Name);
        }

        [Fact]
        public async Task UpdateCustomer_ReturnsNotFound_WhenUpdateFails()
        {
            var dto = new CustomerDto { Name = "Test", Email = "test@example.com" };
            _mockService.Setup(s => s.UpdateCustomerAsync(It.IsAny<Customer>())).ReturnsAsync(false);

            var result = await _controller.UpdateCustomer(dto, 1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateCustomer_ReturnsBadRequest_WhenCustomerIsNull()
        {
            var result = await _controller.UpdateCustomer(null, 1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Customer cannot be null", badRequest.Value);
        }

        [Fact]
        public async Task DeleteCustomer_ReturnsOkResult_WhenDeleteSucceeds()
        {
            _mockService.Setup(s => s.DeleteCustomerAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeleteCustomer(1);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteCustomer_ReturnsNotFound_WhenDeleteFails()
        {
            _mockService.Setup(s => s.DeleteCustomerAsync(1)).ReturnsAsync(false);

            var result = await _controller.DeleteCustomer(1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
