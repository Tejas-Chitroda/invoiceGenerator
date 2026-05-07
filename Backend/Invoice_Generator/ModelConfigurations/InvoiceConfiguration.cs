using Invoice_Generator.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice_Generator.ModelConfigurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> entity)
        {
            entity.Property(e => e.InvoiceDate)
            .IsRequired();

            entity.Property(e => e.SubTotal)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(e => e.TaxTotal)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(e => e.GrandTotal)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
