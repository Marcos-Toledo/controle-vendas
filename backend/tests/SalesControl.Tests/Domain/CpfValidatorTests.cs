using SalesControl.Domain.Validation;

namespace SalesControl.Tests.Domain;

public class CpfValidatorTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("529.982.247-25")]
    [InlineData("11144477735")]
    public void IsValid_ValidCpf_ReturnsTrue(string cpf)
    {
        Assert.True(CpfValidator.IsValid(cpf));
    }

    [Theory]
    [InlineData("12345678900")]
    [InlineData("11111111111")]
    [InlineData("52998224726")]
    [InlineData("123")]
    [InlineData("")]
    [InlineData(null)]
    public void IsValid_InvalidCpf_ReturnsFalse(string? cpf)
    {
        Assert.False(CpfValidator.IsValid(cpf));
    }

    [Fact]
    public void Normalize_RemovesMask()
    {
        Assert.Equal("52998224725", CpfValidator.Normalize("529.982.247-25"));
    }
}
