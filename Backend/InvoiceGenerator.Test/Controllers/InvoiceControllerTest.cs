using Invoice_Generator.Controllers;
using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class InvoiceControllerTests
{
    private readonly Mock<IInvoiceService> _invoiceServiceMock;
    private readonly InvoiceController _controller;

    public InvoiceControllerTests()
    {
        _invoiceServiceMock = new Mock<IInvoiceService>();
        _controller = new InvoiceController(_invoiceServiceMock.Object);
    }

    [Fact]
    public async Task GetAllInvoices_ReturnsOkResultWithInvoices()
    {
        var invoices = new List<Invoice> { new Invoice { Id = 1 }, new Invoice { Id = 2 } };
        _invoiceServiceMock.Setup(s => s.GetAllInvoicesAsync()).ReturnsAsync(invoices);

        var result = await _controller.GetAllInvoices();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnInvoices = Assert.IsAssignableFrom<IEnumerable<Invoice>>(okResult.Value);
        Assert.Equal(2, returnInvoices.Count());
    }

    [Fact]
    public async Task GetInvoiceById_ReturnsNotFound_WhenInvoiceDoesNotExist()
    {
        _invoiceServiceMock.Setup(s => s.GetInvoiceByIdAsync(1)).ReturnsAsync((Invoice)null);

        var result = await _controller.GetInvoiceById(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetInvoiceById_ReturnsOk_WhenInvoiceExists()
    {
        var invoice = new Invoice { Id = 1 };
        _invoiceServiceMock.Setup(s => s.GetInvoiceByIdAsync(1)).ReturnsAsync(invoice);

        var result = await _controller.GetInvoiceById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedInvoice = Assert.IsType<Invoice>(okResult.Value);
        Assert.Equal(1, returnedInvoice.Id);
    }

    [Fact]
    public async Task CreateInvoice_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        _controller.ModelState.AddModelError("CustomerId", "Required");

        var result = await _controller.CreateInvoice(new InvoiceRequestDto());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateInvoice_ReturnsOk_WhenValid()
    {
        var dto = new InvoiceRequestDto { CustomerId = 1 };
        _invoiceServiceMock.Setup(s => s.AddInvoiceAsync(dto)).Returns(Task.CompletedTask);

        var result = await _controller.CreateInvoice(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(dto, okResult.Value);
    }

    [Fact]
    public async Task DeleteInvoice_ReturnsNotFound_WhenInvoiceDoesNotExist()
    {
        _invoiceServiceMock.Setup(s => s.DeleteInvoiceAsync(1)).ReturnsAsync(false);

        var result = await _controller.DeleteInvoice(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteInvoice_ReturnsNoContent_WhenInvoiceDeleted()
    {
        _invoiceServiceMock.Setup(s => s.DeleteInvoiceAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeleteInvoice(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetInvoiceDetailByInvoiceIdAsy_ReturnsBadRequest_WhenIdIsZero()
    {
        var result = await _controller.GetInvoiceDetailByInvoiceIdAsy(0);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("invoiceId must be not null and greater than zero", badRequest.Value);
    }

    [Fact]
    public async Task GetInvoiceDetailByInvoiceIdAsy_ReturnsOk_WithDetails()
    {
        var details = new List<InvoiceDetail> { new InvoiceDetail { InvoiceId = 1 } };
        _invoiceServiceMock.Setup(s => s.GetInvoiceDetailByInvoiceId(1)).ReturnsAsync(details);

        var result = await _controller.GetInvoiceDetailByInvoiceIdAsy(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDetails = Assert.IsAssignableFrom<IEnumerable<InvoiceDetail>>(okResult.Value);
        Assert.Single(returnedDetails);
    }
}
