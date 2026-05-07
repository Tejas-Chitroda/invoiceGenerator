namespace Invoice_Generator.DTOs
{
    public class InvoiceRequestDto
    {
        public int CustomerId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public List<InvoiceItemDto> Items { get; set; } = new List<InvoiceItemDto>();
    }
}
