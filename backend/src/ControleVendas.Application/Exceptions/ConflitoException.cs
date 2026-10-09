namespace ControleVendas.Application.Exceptions;

public class ConflitoException(string message) : AppException(message)
{
    public override int StatusCode => 409;
}
