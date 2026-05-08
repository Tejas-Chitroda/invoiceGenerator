using System.Collections.Generic;
using System.Threading.Tasks;
using Invoice_Generator.DTOs;
using Invoice_Generator.Services.Implementations;
using Xunit;

namespace InvoiceGenerator.Test.Services
{
    public class ProductValidationServiceTest
    {
        private readonly ProductValidationService _service;

        public ProductValidationServiceTest()
        {
            _service = new ProductValidationService();
        }

        [Fact]
        public async Task ValidateProductAsync_NullProduct_ReturnsInvalidResult()
        {
            var result = await _service.ValidateProductAsync(null);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.ContainsKey("Product"));
            Assert.Equal("Product cannot be null", result.Errors["Product"]);
        }

        [Fact]
        public async Task ValidateProductAsync_ValidProduct_ReturnsValidResult()
        {
            var dto = new ProductDto { Name = "Widget", CategoryId = 1, TaxPercentage = 10 };

            var result = await _service.ValidateProductAsync(dto);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ValidateProductAsync_MissingName_ReturnsInvalidResult()
        {
            var dto = new ProductDto { Name = "", CategoryId = 1, TaxPercentage = 10 };

            var result = await _service.ValidateProductAsync(dto);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.ContainsKey("Name"));
            Assert.Equal("Name is required", result.Errors["Name"]);
        }

        [Fact]
        public async Task ValidateProductAsync_WhitespaceName_ReturnsInvalidResult()
        {
            var dto = new ProductDto { Name = "   ", CategoryId = 1, TaxPercentage = 10 };

            var result = await _service.ValidateProductAsync(dto);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.ContainsKey("Name"));
            Assert.Equal("Name is required", result.Errors["Name"]);
        }

        [Fact]
        public async Task ValidateProductAsync_ZeroCategoryId_ReturnsInvalidResult()
        {
            var dto = new ProductDto { Name = "Widget", CategoryId = 0, TaxPercentage = 10 };

            var result = await _service.ValidateProductAsync(dto);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.ContainsKey("CategoryId"));
            Assert.Equal("CategoryId must be a positive integer", result.Errors["CategoryId"]);
        }

        [Fact]
        public async Task ValidateProductAsync_NegativeCategoryId_ReturnsInvalidResult()
        {
            var dto = new ProductDto { Name = "Widget", CategoryId = -1, TaxPercentage = 10 };

            var result = await _service.ValidateProductAsync(dto);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.ContainsKey("CategoryId"));
            Assert.Equal("CategoryId must be a positive integer", result.Errors["CategoryId"]);
        }

        [Fact]
        public async Task ValidateProductAsync_NegativeTaxPercentage_ReturnsInvalidResult()
        {
            var dto = new ProductDto { Name = "Widget", CategoryId = 1, TaxPercentage = -1 };

            var result = await _service.ValidateProductAsync(dto);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.ContainsKey("TaxPercentage"));
            Assert.Equal("TaxPercentage must be between 0 and 100", result.Errors["TaxPercentage"]);
        }

        [Fact]
        public async Task ValidateProductAsync_TaxPercentageOver100_ReturnsInvalidResult()
        {
            var dto = new ProductDto { Name = "Widget", CategoryId = 1, TaxPercentage = 101 };

            var result = await _service.ValidateProductAsync(dto);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.ContainsKey("TaxPercentage"));
            Assert.Equal("TaxPercentage must be between 0 and 100", result.Errors["TaxPercentage"]);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(100)]
        public async Task ValidateProductAsync_ValidTaxPercentage_ReturnsValidResult(decimal tax)
        {
            var dto = new ProductDto { Name = "Widget", CategoryId = 1, TaxPercentage = tax };

            var result = await _service.ValidateProductAsync(dto);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ValidateProductAsync_MultipleErrors_ReturnsAllErrors()
        {
            var dto = new ProductDto { Name = "", CategoryId = 0, TaxPercentage = -5 };

            var result = await _service.ValidateProductAsync(dto);

            Assert.False(result.IsValid);
            Assert.Equal(3, result.Errors.Count);
            Assert.True(result.Errors.ContainsKey("Name"));
            Assert.True(result.Errors.ContainsKey("CategoryId"));
            Assert.True(result.Errors.ContainsKey("TaxPercentage"));
        }
    }
}
