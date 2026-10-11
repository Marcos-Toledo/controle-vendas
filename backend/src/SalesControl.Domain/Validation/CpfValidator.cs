namespace SalesControl.Domain.Validation;

public static class CpfValidator
{
    public static string Normalize(string? cpf)
    {
        return new string((cpf ?? string.Empty).Where(char.IsDigit).ToArray());
    }

    public static bool IsValid(string? cpf)
    {
        var digits = Normalize(cpf);

        if (digits.Length != 11 || digits.Distinct().Count() == 1)
            return false;

        return CheckDigit(digits, 9) == digits[9]
            && CheckDigit(digits, 10) == digits[10];
    }

    private static char CheckDigit(string digits, int position)
    {
        var sum = 0;
        for (var i = 0; i < position; i++)
            sum += (digits[i] - '0') * (position + 1 - i);

        var remainder = (sum * 10) % 11;
        return (char)('0' + (remainder == 10 ? 0 : remainder));
    }
}
