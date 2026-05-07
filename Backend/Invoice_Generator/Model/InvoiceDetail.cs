namespace Invoice_Generator.Models
{
    public class InvoiceDetail
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public decimal Rate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public decimal GrandTotal { get; set; }

        public Invoice? Invoice { get; set; }
        public Product? Product { get; set; }
    }
}
