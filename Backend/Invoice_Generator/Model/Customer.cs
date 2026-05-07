namespace Invoice_Generator.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }

        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
