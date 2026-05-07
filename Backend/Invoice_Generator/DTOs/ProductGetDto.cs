using Invoice_Generator.DTOs;

namespace InvoiceGenerator.DTOs
{
    public class ProductGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal TaxPercentage { get; set; }
        public CategoryGetDto? Category { get; set; }
        public List<ProductPriceGetDto> Prices { get; set; } = new();
    }
}
