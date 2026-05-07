using Invoice_Generator.Controllers;
using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Invoice_Generator.Tests.Controllers
{
    public class ProductPriceControllerTests
    {
        private readonly Mock<IProductPrice> _productPriceServiceMock;
        private readonly ProductPriceController _controller;

        public ProductPriceControllerTests()
        {
            _productPriceServiceMock = new Mock<IProductPrice>();
            _controller = new ProductPriceController(_productPriceServiceMock.Object);
        }

        [Fact]
        public async Task GetAllPrices_ReturnsOkWithPrices()
        {
            var prices = new List<ProductPrice> { new ProductPrice { Id = 1, Price = 100 } };
            _productPriceServiceMock.Setup(p => p.GetAllPriceAsync()).ReturnsAsync(prices);

            var result = await _controller.GetAllPrices();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(prices, okResult.Value);
        }

        [Fact]
        public async Task GetPriceById_ReturnsOk_WhenFound()
        {
            var price = new ProductPrice { Id = 1, Price = 150 };
            _productPriceServiceMock.Setup(p => p.GetPriceByIdAsync(1)).ReturnsAsync(price);

            var result = await _controller.GetPriceById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(price, okResult.Value);
        }

        [Fact]
        public async Task GetPriceById_ReturnsNotFound_WhenNotFound()
        {
            _productPriceServiceMock.Setup(p => p.GetPriceByIdAsync(99)).ReturnsAsync((ProductPrice)null);

            var result = await _controller.GetPriceById(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetPriceByProductId_ReturnsOk_WhenFound()
        {
            var price = new ProductPrice { Id = 1, ProductId = 5, Price = 200 };
            _productPriceServiceMock.Setup(p => p.GetPriceByProductIdAsync(5)).ReturnsAsync(price);

            var result = await _controller.GetPriceByProductId(5);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(price, okResult.Value);
        }

        [Fact]
        public async Task GetPriceByProductId_ReturnsNotFound_WhenNotFound()
        {
            _productPriceServiceMock.Setup(p => p.GetPriceByProductIdAsync(55)).ReturnsAsync((ProductPrice)null);

            var result = await _controller.GetPriceByProductId(55);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddPriceWithDefault_ReturnsBadRequest_IfNull()
        {
            var result = await _controller.AddPriceWithDefault(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Product price cannot be null.", badRequest.Value);
        }

        [Fact]
        public async Task AddPriceWithDefault_CallsServiceAndReturnsOk()
        {
            var priceDto = new ProductPriceDto { ProductId = 1, Price = 100 };

            _productPriceServiceMock.Setup(p => p.AddPriceWithDefaultPriceAsync(priceDto)).Returns(Task.CompletedTask);

            var result = await _controller.AddPriceWithDefault(priceDto);

            _productPriceServiceMock.Verify(p => p.AddPriceWithDefaultPriceAsync(priceDto), Times.Once);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(priceDto, okResult.Value);
        }

        [Fact]
        public async Task UpdatePrice_ReturnsBadRequest_IfIdIsZero()
        {
            var priceDto = new ProductPriceDto { ProductId = 1, Price = 100 };

            var result = await _controller.UpdatePrice(0, priceDto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Product price is invalid.", badRequest.Value);
        }

        [Fact]
        public async Task UpdatePrice_ReturnsNoContent_IfUpdateSucceeds()
        {
            var priceDto = new ProductPriceDto
            {
                ProductId = 1,
                Price = 100,
                EffectiveFrom = null,
                EffectiveTo = null
            };

            _productPriceServiceMock.Setup(p => p.UpdatePriceAsync(It.IsAny<ProductPrice>())).ReturnsAsync(true);

            var result = await _controller.UpdatePrice(1, priceDto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdatePrice_ReturnsNotFound_IfUpdateFails()
        {
            var priceDto = new ProductPriceDto
            {
                ProductId = 1,
                Price = 100
            };

            _productPriceServiceMock.Setup(p => p.UpdatePriceAsync(It.IsAny<ProductPrice>())).ReturnsAsync(false);

            var result = await _controller.UpdatePrice(1, priceDto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeletePrice_ReturnsNoContent_IfDeleted()
        {
            _productPriceServiceMock.Setup(p => p.DeletePriceAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeletePrice(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePrice_ReturnsNotFound_IfDeleteFails()
        {
            _productPriceServiceMock.Setup(p => p.DeletePriceAsync(99)).ReturnsAsync(false);

            var result = await _controller.DeletePrice(99);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
