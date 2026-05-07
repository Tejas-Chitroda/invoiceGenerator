using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invoice_Generator.Models;
using Invoice_Generator.Repository;
using Invoice_Generator.Services.Implementations;
using Invoice_Generator.UoW;
using Moq;
using Xunit;

public class CategoryServiceTest
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<Category>> _categoryRepoMock;
    private readonly CategoryService _service;

    public CategoryServiceTest()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _categoryRepoMock = new Mock<IGenericRepository<Category>>();
        _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
        _service = new CategoryService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AddCategoryAsync_AddsCategoryAndSaves()
    {
        var category = new Category { Id = 1, Name = "Test" };
        _categoryRepoMock.Setup(r => r.AddAsync(category)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        await _service.AddCategoryAsync(category);

        _categoryRepoMock.Verify(r => r.AddAsync(category), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ReturnsAllCategories()
    {
        var categories = new List<Category> { new Category { Id = 1 }, new Category { Id = 2 } };
        _categoryRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        var result = await _service.GetAllCategoriesAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetCategoryByIdAsync_CategoryExists_ReturnsCategory()
    {
        var category = new Category { Id = 1 };
        _categoryRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);

        var result = await _service.GetCategoryByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_CategoryDoesNotExist_ReturnsNull()
    {
        _categoryRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Category?)null);

        var result = await _service.GetCategoryByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateCategoryAsync_CategoryExists_UpdatesAndSaves()
    {
        var category = new Category { Id = 1, Name = "Updated", Description = "desc" };
        var existing = new Category { Id = 1 };
        _categoryRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        var result = await _service.UpdateCategoryAsync(category);

        Assert.True(result);
        _categoryRepoMock.Verify(r => r.Update(existing), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
        Assert.Equal("Updated", existing.Name);
        Assert.Equal("desc", existing.Description);
    }

    [Fact]
    public async Task UpdateCategoryAsync_CategoryDoesNotExist_ReturnsFalse()
    {
        var category = new Category { Id = 1 };
        _categoryRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Category?)null);

        var result = await _service.UpdateCategoryAsync(category);

        Assert.False(result);
        _categoryRepoMock.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteCategoryAsync_CategoryExists_DeletesAndSaves()
    {
        var existing = new Category { Id = 1 };
        _categoryRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveAsync()).ReturnsAsync(1);

        var result = await _service.DeleteCategoryAsync(1);

        Assert.True(result);
        _categoryRepoMock.Verify(r => r.Delete(existing), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCategoryAsync_CategoryDoesNotExist_ReturnsFalse()
    {
        _categoryRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Category?)null);

        var result = await _service.DeleteCategoryAsync(1);

        Assert.False(result);
        _categoryRepoMock.Verify(r => r.Delete(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }
}
