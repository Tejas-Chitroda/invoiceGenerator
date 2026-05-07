namespace Invoice_Generator.DTOs
{
    public class ProductPriceDto
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsDefault { get; set; }
    }
}
