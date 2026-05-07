using Invoice_Generator.DTOs;
using Invoice_Generator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Invoice_Generator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllInvoices()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return Ok(invoices);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            return Ok(invoice);
        }
        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] InvoiceRequestDto invoiceCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _invoiceService.AddInvoiceAsync(invoiceCreateDto);
            return Ok(invoiceCreateDto);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var result = await _invoiceService.DeleteInvoiceAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("getInvoiceDetail/{invoiceId}")]
        public async Task<IActionResult> GetInvoiceDetailByInvoiceIdAsy(int invoiceId)
        {
            if (invoiceId == null || invoiceId == 0)
                return BadRequest("invoiceId must be not null and greater than zero");

            var invoice = await _invoiceService.GetInvoiceDetailByInvoiceId(invoiceId);
            return Ok(invoice);
        }

    }
}
