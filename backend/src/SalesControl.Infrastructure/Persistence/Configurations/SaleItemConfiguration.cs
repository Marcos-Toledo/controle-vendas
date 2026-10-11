using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesControl.Domain.Entities;

namespace SalesControl.Infrastructure.Persistence.Configurations;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("sale_items", t =>
        {
            t.HasCheckConstraint("ck_sale_items_quantity_positive", "quantity > 0");
            t.HasCheckConstraint("ck_sale_items_unit_price_non_negative", "unit_price >= 0");
        });

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(i => i.SaleId).HasColumnName("sale_id").IsRequired();
        builder.Property(i => i.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(i => i.Quantity).HasColumnName("quantity").IsRequired();
        builder.Property(i => i.UnitPrice).HasColumnName("unit_price").HasPrecision(18, 2).IsRequired();
        builder.Property(i => i.ItemTotal)
            .HasColumnName("item_total")
            .HasPrecision(18, 2)
            .HasComputedColumnSql("quantity * unit_price", stored: true);

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => i.SaleId).HasDatabaseName("ix_sale_items_sale");
        builder.HasIndex(i => i.ProductId).HasDatabaseName("ix_sale_items_product");
    }
}
