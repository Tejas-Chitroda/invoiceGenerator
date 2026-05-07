namespace Invoice_Generator.Models
{
    public interface IPriceRangeDto
    {
        DateTime EffectiveFrom { get; set; }
        DateTime EffectiveTo { get; set; }
    }
}
