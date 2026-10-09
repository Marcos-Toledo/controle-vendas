namespace ControleVendas.Application.Exceptions;

public class ValidacaoException(string message) : AppException(message)
{
    public override int StatusCode => 400;
}
