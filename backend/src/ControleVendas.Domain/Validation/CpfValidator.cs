namespace ControleVendas.Domain.Validation;

public static class CpfValidator
{
    public static string Normalizar(string? cpf)
    {
        return new string((cpf ?? string.Empty).Where(char.IsDigit).ToArray());
    }

    public static bool EhValido(string? cpf)
    {
        var digitos = Normalizar(cpf);

        if (digitos.Length != 11 || digitos.Distinct().Count() == 1)
            return false;

        return DigitoVerificador(digitos, 9) == digitos[9]
            && DigitoVerificador(digitos, 10) == digitos[10];
    }

    private static char DigitoVerificador(string digitos, int posicao)
    {
        var soma = 0;
        for (var i = 0; i < posicao; i++)
            soma += (digitos[i] - '0') * (posicao + 1 - i);

        var resto = (soma * 10) % 11;
        return (char)('0' + (resto == 10 ? 0 : resto));
    }
}
