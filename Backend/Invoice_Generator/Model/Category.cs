namespace Invoice_Generator.Models
{
    public class Category
    {
        public int Id { get; set; }  // Primary Key
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Navigation
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
