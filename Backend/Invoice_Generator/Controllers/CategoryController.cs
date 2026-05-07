using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            var category = await _categoryService.GetAllCategoriesAsync();
            return Ok(category);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            
            if(category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDto category)
        {

            if (category == null)
            {
                return BadRequest("Category cannot be null");
            }
            var categoryModel = new Category
            {
                Name = category.Name,
                Description = category.Description
            };
            await _categoryService.AddCategoryAsync(categoryModel);
            return Ok(category);

        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoryById([FromBody] CategoryDto category, int id)
        {
            if (id == 0 || id == null)
            {
                return BadRequest();
            }

            var categoryModel = new Category
            {
                Id = id,             
                Name = category.Name,
                Description = category.Description
            };

            await _categoryService.UpdateCategoryAsync(categoryModel);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategorybyId(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return Ok();
        }

    }
}
