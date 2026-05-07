using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductPriceController : ControllerBase
    {
        private readonly IProductPrice _productPriceService;
        public ProductPriceController(IProductPrice productPriceService)
        {
            _productPriceService = productPriceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPrices()
        {
            var prices = await _productPriceService.GetAllPriceAsync();
            return Ok(prices);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPriceById(int id)
        {
            var price = await _productPriceService.GetPriceByIdAsync(id);
            if (price == null)
            {
                return NotFound();
            }
            return Ok(price);
        }
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetPriceByProductId(int productId)
        {
            var price = await _productPriceService.GetPriceByProductIdAsync(productId);
            if (price == null)
            {
                return NotFound();
            }
            return Ok(price);
        }
      
        [HttpPost("AddPriceDefault")]
        public async Task<IActionResult> AddPriceWithDefault([FromBody] ProductPriceDto productPriceDto)
        {
            if(productPriceDto == null)
            {
                return BadRequest("Product price cannot be null.");
            }

            await _productPriceService.AddPriceWithDefaultPriceAsync(productPriceDto);
            return Ok(productPriceDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrice(int id, [FromBody] ProductPriceDto productPrice)
        {
            if (id <= 0)
            {
                return BadRequest("Product price is invalid.");
            }

            var productPriceModel = new ProductPrice
            {
                Id = id,
                ProductId = productPrice.ProductId,
                Price = productPrice.Price,
                EffectiveFrom = productPrice.EffectiveFrom,
                EffectiveTo = productPrice.EffectiveTo
            };

            var updated = await _productPriceService.UpdatePriceAsync(productPriceModel);
            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrice(int id)
        {
            var deleted = await _productPriceService.DeletePriceAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
