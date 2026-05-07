namespace Invoice_Generator.DTOs
{
    public class ValidationResultDto
    {
        public bool IsValid { get; set; }
        public Dictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
    }
}
