namespace Invoice_Generator.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Constant tax percentage for product
        public decimal TaxPercentage { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // Navigation Property
        public ICollection<ProductPrice> Prices { get; set; }
    }
}
