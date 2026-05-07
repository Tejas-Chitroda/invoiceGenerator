using Invoice_Generator.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice_Generator.ModelConfigurations
{
    public class InvoiceDetailConfiguration : IEntityTypeConfiguration<InvoiceDetail>
    {
        public void Configure(EntityTypeBuilder<InvoiceDetail> entity)
        {
            entity.Property(e => e.Quantity)
                .IsRequired();

            entity.Property(e => e.Rate)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(e => e.SubTotal)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(e => e.Tax)
                .HasPrecision(18, 2)
                .IsRequired();
                  
            entity.Property(e => e.GrandTotal)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.HasOne(e => e.Invoice)
                .WithMany(i => i.InvoiceDetails)
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
