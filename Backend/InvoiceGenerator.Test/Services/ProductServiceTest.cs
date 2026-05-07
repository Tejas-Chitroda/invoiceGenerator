using Invoice_Generator.Models;
using Invoice_Generator.Services.Implementations;
using Invoice_Generator.UoW;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Invoice_Generator.Repository;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<Product>> _productRepoMock;
    private readonly Mock<IGenericRepository<ProductPrice>> _productPriceRepoMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepoMock = new Mock<IGenericRepository<Product>>();
        _productPriceRepoMock = new Mock<IGenericRepository<ProductPrice>>();

        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.ProductPrices).Returns(_productPriceRepoMock.Object);

        _service = new ProductService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetProductByIdAsync_Returns_Product_WhenExists()
    {
        var product = new Product { Id = 1, Name = "Product A" };
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        var result = await _service.GetProductByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Product A", result.Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_Returns_Null_WhenNotExists()
    {
        _productRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product)null);

        var result = await _service.GetProductByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddProductAsync_AddsProduct_AndSaves()
    {
        var product = new Product { Id = 1, Name = "New Product" };

        await _service.AddProductAsync(product);

        _productRepoMock.Verify(r => r.AddAsync(product), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Updates_IfProductExists()
    {
        var existing = new Product { Id = 1, Name = "Old", Description = "Old Desc" };
        var updated = new Product { Id = 1, Name = "New", Description = "New Desc" };

        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        var result = await _service.UpdateAsync(updated);

        Assert.True(result);
        Assert.Equal("New", existing.Name);
        Assert.Equal("New Desc", existing.Description);
        _productRepoMock.Verify(r => r.Update(existing), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenProductNotExists()
    {
        var updated = new Product { Id = 1, Name = "New" };
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product)null);

        var result = await _service.UpdateAsync(updated);

        Assert.False(result);
        _productRepoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_Deletes_IfProductExists()
    {
        var existing = new Product { Id = 1 };
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
        _productRepoMock.Verify(r => r.Delete(existing), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_IfProductNotExists()
    {
        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product)null);

        var result = await _service.DeleteAsync(1);

        Assert.False(result);
        _productRepoMock.Verify(r => r.Delete(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task GetTodaysPriceAsync_ReturnsCurrentPrice_WhenWithinDateRange()
    {
        var productId = 1;
        var today = DateTime.UtcNow;
        var price = new ProductPrice
        {
            ProductId = productId,
            Price = 150,
            IsDefault = false,
            EffectiveFrom = today.AddDays(-1),
            EffectiveTo = today.AddDays(1)
        };

        _productPriceRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductPrice, bool>>>()))
            .ReturnsAsync(new List<ProductPrice> { price });

        var result = await _service.GetTodaysPriceAsync(productId);

        Assert.Equal(150, result);
    }

    [Fact]
    public async Task GetTodaysPriceAsync_ReturnsDefaultPrice_WhenNoDatePrice()
    {
        var productId = 1;
        var defaultPrice = new ProductPrice
        {
            ProductId = productId,
            Price = 100,
            IsDefault = true
        };

        // First call returns default price
        _productPriceRepoMock.SetupSequence(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductPrice, bool>>>()))
            .ReturnsAsync(new List<ProductPrice> { defaultPrice }) // default price
            .ReturnsAsync(new List<ProductPrice>()); // no valid price for today

        var result = await _service.GetTodaysPriceAsync(productId);

        Assert.Equal(100, result);
    }

    [Fact]
    public async Task GetTodaysPriceAsync_ReturnsNull_WhenNoPriceAvailable()
    {
        var productId = 1;
        _productPriceRepoMock.SetupSequence(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductPrice, bool>>>()))
            .ReturnsAsync(new List<ProductPrice>()) // default price
            .ReturnsAsync(new List<ProductPrice>()); // today's price

        var result = await _service.GetTodaysPriceAsync(productId);

        Assert.Null(result);
    }
}
