using ControleVendas.Domain.Validation;

namespace ControleVendas.Tests.Domain;

public class CpfValidatorTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("529.982.247-25")]
    [InlineData("11144477735")]
    public void EhValido_CpfValido_RetornaTrue(string cpf)
    {
        Assert.True(CpfValidator.EhValido(cpf));
    }

    [Theory]
    [InlineData("12345678900")]
    [InlineData("11111111111")]
    [InlineData("52998224726")]
    [InlineData("123")]
    [InlineData("")]
    [InlineData(null)]
    public void EhValido_CpfInvalido_RetornaFalse(string? cpf)
    {
        Assert.False(CpfValidator.EhValido(cpf));
    }

    [Fact]
    public void Normalizar_RemoveMascara()
    {
        Assert.Equal("52998224725", CpfValidator.Normalizar("529.982.247-25"));
    }
}
