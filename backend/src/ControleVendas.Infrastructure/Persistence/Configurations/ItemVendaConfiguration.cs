using ControleVendas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleVendas.Infrastructure.Persistence.Configurations;

public class ItemVendaConfiguration : IEntityTypeConfiguration<ItemVenda>
{
    public void Configure(EntityTypeBuilder<ItemVenda> builder)
    {
        builder.ToTable("itens_venda", t =>
        {
            t.HasCheckConstraint("ck_itens_quantidade_positiva", "quantidade > 0");
            t.HasCheckConstraint("ck_itens_preco_positivo", "preco_unitario >= 0");
        });

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(i => i.VendaId).HasColumnName("venda_id").IsRequired();
        builder.Property(i => i.ProdutoId).HasColumnName("produto_id").IsRequired();
        builder.Property(i => i.Quantidade).HasColumnName("quantidade").IsRequired();
        builder.Property(i => i.PrecoUnitario).HasColumnName("preco_unitario").HasPrecision(18, 2).IsRequired();
        builder.Property(i => i.TotalItem)
            .HasColumnName("total_item")
            .HasPrecision(18, 2)
            .HasComputedColumnSql("quantidade * preco_unitario", stored: true);

        builder.HasOne(i => i.Produto)
            .WithMany()
            .HasForeignKey(i => i.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => i.VendaId).HasDatabaseName("ix_itens_venda");
        builder.HasIndex(i => i.ProdutoId).HasDatabaseName("ix_itens_produto");
    }
}
