using ControleVendas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleVendas.Infrastructure.Persistence.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("produtos");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(p => p.Nome).HasColumnName("nome").HasMaxLength(200).IsRequired();
        builder.Property(p => p.ValorUnitario).HasColumnName("valor_unitario").HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.CriadoEm).HasColumnName("criado_em").HasDefaultValueSql("now()");
        builder.Property(p => p.AtualizadoEm).HasColumnName("atualizado_em").HasDefaultValueSql("now()");

        builder.HasIndex(p => p.Nome).IsUnique().HasDatabaseName("ux_produtos_nome");
    }
}
