using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Invoice_Generator.Models;

namespace Invoice_Generator.ModelConfigurations
{
    public class ProductPriceConfiguration : IEntityTypeConfiguration<ProductPrice>
    {
        public void Configure(EntityTypeBuilder<ProductPrice> entity)
        {
            entity.Property(e => e.Price)
            .HasPrecision(18, 2)
            .IsRequired();

            entity.Property(e => e.EffectiveFrom)
                .IsRequired(false)
                .HasColumnType("datetime");

            entity.Property(e => e.EffectiveTo)
                .IsRequired(false)
                .HasColumnType("datetime");

            entity.Property(e => e.IsDefault)
                .IsRequired();

            entity.HasOne(e => e.Product)
                .WithMany(p => p.Prices)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
