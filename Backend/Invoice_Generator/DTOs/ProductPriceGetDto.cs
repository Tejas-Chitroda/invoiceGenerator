namespace InvoiceGenerator.DTOs
{
    public class ProductPriceGetDto
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
