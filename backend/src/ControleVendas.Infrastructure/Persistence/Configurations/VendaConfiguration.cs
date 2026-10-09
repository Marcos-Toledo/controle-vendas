using ControleVendas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleVendas.Infrastructure.Persistence.Configurations;

public class VendaConfiguration : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.ToTable("vendas");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(v => v.ClienteId).HasColumnName("cliente_id").IsRequired();
        builder.Property(v => v.DataVenda).HasColumnName("data_venda").HasDefaultValueSql("CURRENT_DATE").IsRequired();
        builder.Property(v => v.ValorTotal).HasColumnName("valor_total").HasPrecision(18, 2).HasDefaultValue(0m);
        builder.Property(v => v.CriadoEm).HasColumnName("criado_em").HasDefaultValueSql("now()");

        builder.HasOne(v => v.Cliente)
            .WithMany()
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(v => v.Itens)
            .WithOne()
            .HasForeignKey(i => i.VendaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(v => v.ClienteId).HasDatabaseName("ix_vendas_cliente");
        builder.HasIndex(v => v.DataVenda).HasDatabaseName("ix_vendas_data");
    }
}
