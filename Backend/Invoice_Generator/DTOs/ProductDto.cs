namespace Invoice_Generator.DTOs
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // Constant tax percentage for product
        public decimal TaxPercentage { get; set; }
        public int CategoryId { get; set; }
    }
}
