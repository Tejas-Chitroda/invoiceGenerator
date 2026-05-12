using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_Generator.Controllers
{
    /// <summary>
    /// Handles CRUD operations for products and returns today's price for a product.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductValidationService _validationService;

        public ProductController(IProductService productService, IProductValidationService validationService)
        {
            _productService = productService;
            _validationService = validationService;
        }

        /// <summary>
        /// Gets all products.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllProduct()
        {
            var products = await _productService.GetAllProductAsync();
            return Ok(products);
        }

        /// <summary>
        /// Gets a product by id.
        /// </summary>
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

        /// <summary>
        /// Creates a new product after validation.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto product)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null");
            }

            // Validate product
            var validationResult = await _validationService.ValidateProductAsync(product);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
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

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductById(int id, [FromBody] ProductDto product)
        {
            if (id == 0)
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

        /// <summary>
        /// Deletes a product by id.
        /// </summary>
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

        /// <summary>
        /// Gets today's price for a product.
        /// </summary>
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
