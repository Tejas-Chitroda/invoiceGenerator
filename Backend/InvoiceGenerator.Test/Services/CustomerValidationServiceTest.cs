using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Repository;
using Invoice_Generator.Services.Implementations;
using Invoice_Generator.UoW;
using Moq;
using Xunit;

namespace InvoiceGenerator.Test.Services
{
    public class CustomerValidationServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IGenericRepository<Customer>> _customerRepoMock;
        private readonly CustomerValidationService _service;

        public CustomerValidationServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _customerRepoMock = new Mock<IGenericRepository<Customer>>();
            _unitOfWorkMock.Setup(u => u.Customers).Returns(_customerRepoMock.Object);
            _service = new CustomerValidationService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task ValidateCustomerAsync_NullCustomer_ReturnsInvalidResult()
        {
            var result = await _service.ValidateCustomerAsync(null);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.ContainsKey("Customer"));
            Assert.Equal("Customer cannot be null", result.Errors["Customer"]);
            _customerRepoMock.Verify(
                r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Customer, bool>>>()),
                Times.Never);
        }

        [Fact]
        public async Task ValidateCustomerAsync_ValidCustomer_ReturnsValidResult()
        {
            var dto = new CustomerDto { Name = "Test User", Email = "test@example.com" };
            _customerRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Customer, bool>>>()))
                .ReturnsAsync(new List<Customer>());

            var result = await _service.ValidateCustomerAsync(dto);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ValidateCustomerAsync_MissingName_ReturnsInvalidResult()
        {
            var dto = new CustomerDto { Name = "", Email = "test@example.com" };
            _customerRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Customer, bool>>>()))
                .ReturnsAsync(new List<Customer>());

            var result = await _service.ValidateCustomerAsync(dto);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.ContainsKey("Name"));
            Assert.Equal("Name is required", result.Errors["Name"]);
        }

        [Fact]
        public async Task ValidateCustomerAsync_MissingEmail_ReturnsInvalidResult()
        {
            var dto = new CustomerDto { Name = "Test User", Email = "" };

            var result = await _service.ValidateCustomerAsync(dto);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.ContainsKey("Email"));
            Assert.Equal("Email is required", result.Errors["Email"]);
        }

        [Fact]
        public async Task ValidateCustomerAsync_InvalidEmailFormat_ReturnsInvalidResult()
        {
            var dto = new CustomerDto { Name = "Test User", Email = "invalid-email" };
            _customerRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Customer, bool>>>()))
                .ReturnsAsync(new List<Customer>());

            var result = await _service.ValidateCustomerAsync(dto);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.ContainsKey("Email"));
            Assert.Equal("Invalid email format", result.Errors["Email"]);
        }

        [Fact]
        public async Task ValidateCustomerAsync_DuplicateEmail_ReturnsInvalidResult()
        {
            var dto = new CustomerDto { Name = "Test User", Email = "existing@example.com" };
            var existingCustomer = new Customer { Id = 1, Name = "Existing", Email = "existing@example.com" };
            _customerRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Customer, bool>>>()))
                .ReturnsAsync(new List<Customer> { existingCustomer });

            var result = await _service.ValidateCustomerAsync(dto);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.ContainsKey("Email"));
            Assert.Equal("Email already exists", result.Errors["Email"]);
        }

        [Fact]
        public async Task ValidateCustomerAsync_DuplicateEmailDifferentCase_ReturnsInvalidResult()
        {
            var dto = new CustomerDto { Name = "Test User", Email = "Existing@Example.COM" };
            var existingCustomer = new Customer { Id = 1, Name = "Existing", Email = "existing@example.com" };
            _customerRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Customer, bool>>>()))
                .ReturnsAsync(new List<Customer> { existingCustomer });

            var result = await _service.ValidateCustomerAsync(dto);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.ContainsKey("Email"));
            Assert.Equal("Email already exists", result.Errors["Email"]);
        }

        [Fact]
        public async Task ValidateCustomerAsync_MultipleErrors_ReturnsAllErrors()
        {
            var dto = new CustomerDto { Name = "", Email = "" };

            var result = await _service.ValidateCustomerAsync(dto);

            Assert.False(result.IsValid);
            Assert.Equal(2, result.Errors.Count);
            Assert.True(result.Errors.ContainsKey("Name"));
            Assert.True(result.Errors.ContainsKey("Email"));
        }

        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user.name@example.co.uk")]
        [InlineData("user+tag@domain.com")]
        public async Task ValidateCustomerAsync_ValidEmailFormats_ReturnsValidResult(string email)
        {
            var dto = new CustomerDto { Name = "Test User", Email = email };
            _customerRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Customer, bool>>>()))
                .ReturnsAsync(new List<Customer>());

            var result = await _service.ValidateCustomerAsync(dto);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("@example.com")]
        [InlineData("user@")]
        [InlineData("user @example.com")]
        public async Task ValidateCustomerAsync_InvalidEmailFormats_ReturnsInvalidResult(string email)
        {
            var dto = new CustomerDto { Name = "Test User", Email = email };
            _customerRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Customer, bool>>>()))
                .ReturnsAsync(new List<Customer>());

            var result = await _service.ValidateCustomerAsync(dto);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.ContainsKey("Email"));
            Assert.Equal("Invalid email format", result.Errors["Email"]);
        }
    }
}
