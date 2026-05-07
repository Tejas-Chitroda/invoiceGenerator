using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invoice_Generator.Models;
using Invoice_Generator.Repository;
using Invoice_Generator.Services.Implementations;
using Invoice_Generator.UoW;
using Moq;
using Xunit;

public class CustomerServiceTest
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<Customer>> _customerRepoMock;
    private readonly CustomerService _service;

    public CustomerServiceTest()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _customerRepoMock = new Mock<IGenericRepository<Customer>>();
        _unitOfWorkMock.Setup(u => u.Customers).Returns(_customerRepoMock.Object);
        _service = new CustomerService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddCustomerAsync_AddsCustomerAndSaves()
    {
        var customer = new Customer { Id = 1, Name = "Test", Email = "test@example.com" };
        _customerRepoMock.Setup(r => r.AddAsync(customer)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        await _service.AddCustomerAsync(customer);

        _customerRepoMock.Verify(r => r.AddAsync(customer), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCustomerAsync_CustomerExists_DeletesAndSaves()
    {
        var customer = new Customer { Id = 1 };
        _customerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        var result = await _service.DeleteCustomerAsync(1);

        Assert.True(result);
        _customerRepoMock.Verify(r => r.Delete(customer), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCustomerAsync_CustomerDoesNotExist_ReturnsFalse()
    {
        _customerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Customer?)null);

        var result = await _service.DeleteCustomerAsync(1);

        Assert.False(result);
        _customerRepoMock.Verify(r => r.Delete(It.IsAny<Customer>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAllCustomersAsync_ReturnsAllCustomers()
    {
        var customers = new List<Customer> { new Customer { Id = 1 }, new Customer { Id = 2 } };
        _customerRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);

        var result = await _service.GetAllCustomersAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetCustomerByIdAsync_CustomerExists_ReturnsCustomer()
    {
        var customer = new Customer { Id = 1 };
        _customerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);

        var result = await _service.GetCustomerByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_CustomerDoesNotExist_ReturnsNull()
    {
        _customerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Customer?)null);

        var result = await _service.GetCustomerByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateCustomerAsync_CustomerExists_UpdatesAndSaves()
    {
        var customer = new Customer { Id = 1, Name = "Updated", Email = "updated@example.com" };
        var existing = new Customer { Id = 1 };
        _customerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        var result = await _service.UpdateCustomerAsync(customer);

        Assert.True(result);
        _customerRepoMock.Verify(r => r.Update(existing), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
        Assert.Equal("Updated", existing.Name);
        Assert.Equal("updated@example.com", existing.Email);
    }

    [Fact]
    public async Task UpdateCustomerAsync_CustomerDoesNotExist_ReturnsFalse()
    {
        var customer = new Customer { Id = 1 };
        _customerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Customer?)null);

        var result = await _service.UpdateCustomerAsync(customer);

        Assert.False(result);
        _customerRepoMock.Verify(r => r.Update(It.IsAny<Customer>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }
}
