using ControleVendas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleVendas.Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("clientes");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(c => c.Cpf).HasColumnName("cpf").HasMaxLength(11).IsRequired();
        builder.Property(c => c.Nome).HasColumnName("nome").HasMaxLength(200).IsRequired();
        builder.Property(c => c.DataNascimento).HasColumnName("data_nascimento");
        builder.Property(c => c.Email).HasColumnName("email").HasMaxLength(200);
        builder.Property(c => c.Telefone).HasColumnName("telefone").HasMaxLength(20);
        builder.Property(c => c.CriadoEm).HasColumnName("criado_em").HasDefaultValueSql("now()");
        builder.Property(c => c.AtualizadoEm).HasColumnName("atualizado_em").HasDefaultValueSql("now()");

        builder.HasIndex(c => c.Cpf).IsUnique().HasDatabaseName("ux_clientes_cpf");
        builder.HasIndex(c => c.Nome).HasDatabaseName("ix_clientes_nome");
    }
}
