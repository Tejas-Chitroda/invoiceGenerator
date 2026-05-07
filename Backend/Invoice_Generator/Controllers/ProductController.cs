using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProduct()
        {
            var products = await _productService.GetAllProductAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto product)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null");
            }
            var productModel = new Product
            {
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                TaxPercentage = product.TaxPercentage
            };

            await _productService.AddProductAsync(productModel);
            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductById(int id, [FromBody] ProductDto product)
        {
            if (id == null || id == 0)
            {
                return BadRequest("Product data is invalid");
            }

            var productModel = new Product
            {
                Id = id,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                TaxPercentage = product.TaxPercentage
            };

            var updated = await _productService.UpdateAsync(productModel);
            if (!updated)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductById(int id)
        {
            var deleted = await _productService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("price/{productId}")]
        public async Task<IActionResult> GetPriceForToday(int productId)
        {
            var price = await _productService.GetTodaysPriceAsync(productId);
            if (price == null)
            {
                return NotFound("Price not found for today");
            }
            return Ok(price);

        }
    }
}
