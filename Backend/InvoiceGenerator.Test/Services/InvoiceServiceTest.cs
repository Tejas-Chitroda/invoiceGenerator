using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Repository;
using Invoice_Generator.Services.Implementations;
using Invoice_Generator.UoW;
using Moq;
using System.Linq.Expressions;
using Xunit;

public class InvoiceServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<Invoice>> _invoiceRepoMock;
    private readonly Mock<IGenericRepository<InvoiceDetail>> _invoiceDetailRepoMock;
    private readonly Mock<IGenericRepository<Product>> _productRepoMock;
    private readonly Mock<IGenericRepository<ProductPrice>> _productPriceRepoMock;
    private readonly InvoiceService _service;

    public InvoiceServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _invoiceRepoMock = new Mock<IGenericRepository<Invoice>>();
        _invoiceDetailRepoMock = new Mock<IGenericRepository<InvoiceDetail>>();
        _productRepoMock = new Mock<IGenericRepository<Product>>();
        _productPriceRepoMock = new Mock<IGenericRepository<ProductPrice>>();

        _unitOfWorkMock.Setup(u => u.Invoices).Returns(_invoiceRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.InvoiceDetails).Returns(_invoiceDetailRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.ProductPrices).Returns(_productPriceRepoMock.Object);

        _service = new InvoiceService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddInvoiceAsync_ThrowsIfProductDoesNotExist()
    {
        var request = new InvoiceRequestDto
        {
            CustomerId = 1,
            Items = new List<InvoiceItemDto> { new InvoiceItemDto { ProductId = 1, Quantity = 1 } }
        };

        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product)null);

        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddInvoiceAsync(request));
    }

    [Fact]
    public async Task AddInvoiceAsync_AddsInvoiceAndDetailsSuccessfully()
    {
        var product = new Product { Id = 1, TaxPercentage = 10 };
        var price = new ProductPrice { Price = 100, IsDefault = true };

        _productRepoMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(product);
        _productPriceRepoMock.Setup(p => p.FindAsync(It.IsAny<Expression<Func<ProductPrice, bool>>>()))
            .ReturnsAsync(new List<ProductPrice> { price });

        var request = new InvoiceRequestDto
        {
            CustomerId = 1,
            Items = new List<InvoiceItemDto> { new InvoiceItemDto { ProductId = 1, Quantity = 2 } }
        };

        await _service.AddInvoiceAsync(request);

        _invoiceRepoMock.Verify(r => r.AddAsync(It.IsAny<Invoice>()), Times.Once);
        _invoiceDetailRepoMock.Verify(r => r.AddAsync(It.IsAny<InvoiceDetail>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteInvoiceAsync_ReturnsFalse_WhenInvoiceDoesNotExist()
    {
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Invoice)null);

        var result = await _service.DeleteInvoiceAsync(1);

        Assert.False(result);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteInvoiceAsync_DeletesInvoice_WhenExists()
    {
        var invoice = new Invoice { Id = 1 };
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(invoice);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        var result = await _service.DeleteInvoiceAsync(1);

        Assert.True(result);
        _invoiceRepoMock.Verify(r => r.Delete(invoice), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllInvoicesAsync_ReturnsInvoices()
    {
        var invoices = new List<Invoice> { new Invoice(), new Invoice() };
        _invoiceRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(invoices);

        var result = await _service.GetAllInvoicesAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetInvoiceByIdAsync_ReturnsInvoice()
    {
        var invoice = new Invoice { Id = 1 };
        _invoiceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(invoice);

        var result = await _service.GetInvoiceByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetInvoiceDetailByInvoiceId_ReturnsDetails()
    {
        var details = new List<InvoiceDetail> { new InvoiceDetail { InvoiceId = 1 } };
        _invoiceDetailRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<InvoiceDetail, bool>>>()))
            .ReturnsAsync(details);

        var result = await _service.GetInvoiceDetailByInvoiceId(1);

        Assert.Single(result);
    }
}
