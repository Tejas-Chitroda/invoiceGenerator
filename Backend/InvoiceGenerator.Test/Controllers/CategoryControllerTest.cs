using Invoice_Generator.Controllers;
using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceGenerator.Test.Controllers
{
    public class CategoryControllerTest
    {
        private readonly Mock<ICategoryService> _categoryServiceMock;
        private readonly CategoryController _categoryController;

        public CategoryControllerTest()
        {
            _categoryServiceMock = new Mock<ICategoryService>();
            _categoryController = new CategoryController(_categoryServiceMock.Object);
        }

        [Fact]
        public async Task GetAllCategory_ReturnsOkWithCategories()
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1", Description = "Description1" },
                new Category { Id = 2, Name = "Category2", Description = "Description2" }
            };

            _categoryServiceMock.Setup(s => s.GetAllCategoriesAsync())
                .ReturnsAsync(categories);

            var result = await _categoryController.GetAllCategory();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(categories, okResult.Value);

        }

        [Fact]
        public async Task GetCategoryById_CategoryExists_ReturnsOkWithCategory()
        {
            var category = new Category { Id = 1 };
            _categoryServiceMock.Setup(s => s.GetCategoryByIdAsync(1)).ReturnsAsync(category);

            var result = await _categoryController.GetCategoryById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(category, okResult.Value);
        }

        [Fact]
        public async Task GetCategoryById_CategoryDoesNotExist_ReturnsNotFound()
        {
            _categoryServiceMock.Setup(s => s.GetCategoryByIdAsync(1)).ReturnsAsync((Category?)null);

            var result = await _categoryController.GetCategoryById(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddCategory_NullCategory_ReturnsBadRequest()
        {
            var result = await _categoryController.AddCategory(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Category cannot be null", badRequest.Value);
        }

        [Fact]
        public async Task AddCategory_ValidCategory_CallsServiceAndReturnsOk()
        {
            var dto = new CategoryDto { Name = "Test", Description = "Desc" };

            var result = await _categoryController.AddCategory(dto);

            _categoryServiceMock.Verify(s => s.AddCategoryAsync(It.Is<Category>(c => c.Name == dto.Name && c.Description == dto.Description)), Times.Once);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, okResult.Value);
        }

        [Fact]
        public async Task UpdateCategoryById_IdIsDefault_ReturnsBadRequest()
        {
            var dto = new CategoryDto { Name = "Test", Description = "Desc" };

            var result = await _categoryController.UpdateCategoryById(dto, 0);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateCategoryById_Valid_CallsServiceAndReturnsOk()
        {
            var dto = new CategoryDto { Name = "Test", Description = "Desc" };

            var result = await _categoryController.UpdateCategoryById(dto, 1);

            _categoryServiceMock.Verify(s => s.UpdateCategoryAsync(It.Is<Category>(c => c.Id == 1 && c.Name == dto.Name && c.Description == dto.Description)), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteCategorybyId_CallsServiceAndReturnsOk()
        {
            _categoryServiceMock.Setup(s => s.DeleteCategoryAsync(1)).ReturnsAsync(true);

            var result = await _categoryController.DeleteCategorybyId(1);

            _categoryServiceMock.Verify(s => s.DeleteCategoryAsync(1), Times.Once);
            Assert.IsType<OkResult>(result);
        }
    }
}
