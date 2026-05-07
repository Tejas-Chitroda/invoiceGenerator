using Invoice_Generator.Controllers;
using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using InvoiceGenerator.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Invoice_Generator.Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _productServiceMock;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _productServiceMock = new Mock<IProductService>();
            _controller = new ProductController(_productServiceMock.Object);
        }

        [Fact]
        public async Task GetAllProduct_ReturnsOkWithProducts()
        {
            var products = new List<ProductGetDto> { new ProductGetDto { Id = 1, Name = "P1" } };
            _productServiceMock.Setup(s => s.GetAllProductAsync()).ReturnsAsync(products);

            var result = await _controller.GetAllProduct();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(products, okResult.Value);
        }

        [Fact]
        public async Task GetProductById_ProductExists_ReturnsOk()
        {
            var product = new Product { Id = 1, Name = "P1" };
            _productServiceMock.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(product);

            var result = await _controller.GetProductById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(product, okResult.Value);
        }

        [Fact]
        public async Task GetProductById_ProductNotExists_ReturnsNotFound()
        {
            _productServiceMock.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync((Product)null);

            var result = await _controller.GetProductById(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddProduct_ValidProduct_ReturnsOk()
        {
            var productDto = new ProductDto { Name = "P1", CategoryId = 1, TaxPercentage = 5 };

            var result = await _controller.AddProduct(productDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(productDto, okResult.Value);
            _productServiceMock.Verify(s => s.AddProductAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task AddProduct_NullProduct_ReturnsBadRequest()
        {
            var result = await _controller.AddProduct(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Product cannot be null", badRequest.Value);
        }

        [Fact]
        public async Task UpdateProductById_ValidUpdate_ReturnsOk()
        {
            var productDto = new ProductDto { Name = "Updated", CategoryId = 2, TaxPercentage = 5 };
            _productServiceMock.Setup(s => s.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(true);

            var result = await _controller.UpdateProductById(1, productDto);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task UpdateProductById_InvalidId_ReturnsBadRequest()
        {
            var productDto = new ProductDto { Name = "Updated", CategoryId = 2, TaxPercentage = 5 };

            var result = await _controller.UpdateProductById(0, productDto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Product data is invalid", badRequest.Value);
        }

        [Fact]
        public async Task UpdateProductById_ProductNotFound_ReturnsNotFound()
        {
            var productDto = new ProductDto { Name = "Updated", CategoryId = 2, TaxPercentage = 5 };
            _productServiceMock.Setup(s => s.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(false);

            var result = await _controller.UpdateProductById(1, productDto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteProductById_ProductExists_ReturnsNoContent()
        {
            _productServiceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeleteProductById(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProductById_ProductNotExists_ReturnsNotFound()
        {
            _productServiceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

            var result = await _controller.DeleteProductById(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetPriceForToday_PriceFound_ReturnsOk()
        {
            decimal price = 99.99m;
            _productServiceMock.Setup(s => s.GetTodaysPriceAsync(1)).ReturnsAsync(price);

            var result = await _controller.GetPriceForToday(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(price, okResult.Value);
        }

        [Fact]
        public async Task GetPriceForToday_PriceNotFound_ReturnsNotFound()
        {
            _productServiceMock.Setup(s => s.GetTodaysPriceAsync(1)).ReturnsAsync((decimal?)null);

            var result = await _controller.GetPriceForToday(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Price not found for today", notFound.Value);
        }
    }
}
