namespace Invoice_Generator.Models
{
    public class ProductPrice
    {
        public int Id { get; set; }  // Primary Key
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsDefault { get; set; }

        // Navigation
        public Product? Product { get; set; }
    }
}
