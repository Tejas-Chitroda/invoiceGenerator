using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Services.Implementations;
using Invoice_Generator.UoW;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Invoice_Generator.Repository;

public class ProductPriceServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<ProductPrice>> _productPriceRepoMock;
    private readonly Mock<IGenericRepository<Product>> _productRepoMock;
    private readonly IProductPriceService _service;

    public ProductPriceServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productPriceRepoMock = new Mock<IGenericRepository<ProductPrice>>();
        _productRepoMock = new Mock<IGenericRepository<Product>>();

        _unitOfWorkMock.Setup(u => u.ProductPrices).Returns(_productPriceRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);

        _service = new IProductPriceService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddPriceAsync_AddsPrice_AndSaves()
    {
        var price = new ProductPrice { ProductId = 1, Price = 100 };

        await _service.AddPriceAsync(price);

        _productPriceRepoMock.Verify(p => p.AddAsync(price), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task AddPriceWithDefaultPriceAsync_ThrowsIfProductNotFound()
    {
        var dto = new ProductPriceDto { ProductId = 1, IsDefault = true, Price = 100 };
        _productRepoMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync((Product)null);

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.AddPriceWithDefaultPriceAsync(dto));
        Assert.Equal("Product Not Found", ex.Message);
    }

    [Fact]
    public async Task AddPriceWithDefaultPriceAsync_ThrowsIfDefaultPriceAlreadyExists()
    {
        var dto = new ProductPriceDto { ProductId = 1, IsDefault = true, Price = 100 };
        _productRepoMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(new Product { Id = 1 });

        _productPriceRepoMock
            .Setup(p => p.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductPrice, bool>>>()))
            .ReturnsAsync(new List<ProductPrice> { new ProductPrice() });

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.AddPriceWithDefaultPriceAsync(dto));
        Assert.Equal("Default price already exists for this product.", ex.Message);
    }

    [Fact]
    public async Task AddPriceWithDefaultPriceAsync_AddsDefaultPrice_WhenNotExists()
    {
        var dto = new ProductPriceDto { ProductId = 1, IsDefault = true, Price = 100 };
        _productRepoMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(new Product { Id = 1 });

        _productPriceRepoMock
            .Setup(p => p.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductPrice, bool>>>()))
            .ReturnsAsync(new List<ProductPrice>());

        await _service.AddPriceWithDefaultPriceAsync(dto);

        _productPriceRepoMock.Verify(p => p.AddAsync(It.Is<ProductPrice>(pp => pp.IsDefault && pp.Price == 100)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }


    [Fact]
    public async Task AddPriceWithDefaultPriceAsync_ThrowsIfEffectiveDatesMissing_ForNonDefault()
    {
        var dto = new ProductPriceDto { ProductId = 1, IsDefault = false, Price = 100, EffectiveFrom = null, EffectiveTo = null };
        _productRepoMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(new Product { Id = 1 });

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.AddPriceWithDefaultPriceAsync(dto));
        Assert.Equal("Effective dates must be provided for non-default prices.", ex.Message);
    }

    [Fact]
    public async Task AddPriceWithDefaultPriceAsync_ThrowsIfEffectiveFromGreaterThanTo()
    {
        var dto = new ProductPriceDto
        {
            ProductId = 1,
            IsDefault = false,
            Price = 100,
            EffectiveFrom = DateTime.UtcNow.AddDays(5),
            EffectiveTo = DateTime.UtcNow
        };
        _productRepoMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(new Product { Id = 1 });

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.AddPriceWithDefaultPriceAsync(dto));
        Assert.Equal("Effective From date must be earlier than Effective To date.", ex.Message);
    }

    [Fact]
    public async Task DeletePriceAsync_ReturnsFalse_WhenNotExists()
    {
        _productPriceRepoMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync((ProductPrice)null);

        var result = await _service.DeletePriceAsync(1);

        Assert.False(result);
        _productPriceRepoMock.Verify(p => p.Delete(It.IsAny<ProductPrice>()), Times.Never);
    }

    [Fact]
    public async Task DeletePriceAsync_Deletes_WhenExists()
    {
        var price = new ProductPrice { Id = 1 };
        _productPriceRepoMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(price);

        var result = await _service.DeletePriceAsync(1);

        Assert.True(result);
        _productPriceRepoMock.Verify(p => p.Delete(price), Times.Once);
    }

    [Fact]
    public async Task GetAllPriceAsync_ReturnsAllPrices()
    {
        var prices = new List<ProductPrice> { new ProductPrice { Id = 1 }, new ProductPrice { Id = 2 } };
        _productPriceRepoMock.Setup(p => p.GetAllAsync()).ReturnsAsync(prices);

        var result = await _service.GetAllPriceAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetPriceByIdAsync_ReturnsCorrectPrice()
    {
        var price = new ProductPrice { Id = 1 };
        _productPriceRepoMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(price);

        var result = await _service.GetPriceByIdAsync(1);

        Assert.Equal(price, result);
    }

    [Fact]
    public async Task UpdatePriceAsync_UpdatesAndSaves()
    {
        var price = new ProductPrice
        {
            Id = 1,
            Price = 200,
            ProductId = 5,
            EffectiveFrom = DateTime.UtcNow,
            EffectiveTo = DateTime.UtcNow.AddDays(10)
        };
        var existing = new ProductPrice { Id = 1 };
        _productPriceRepoMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(existing);

        var result = await _service.UpdatePriceAsync(price);

        Assert.True(result);
        Assert.Equal(200, existing.Price);
        Assert.Equal(5, existing.ProductId);
        _productPriceRepoMock.Verify(p => p.Update(existing), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }
}
