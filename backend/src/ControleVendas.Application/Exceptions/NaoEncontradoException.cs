namespace ControleVendas.Application.Exceptions;

public class NaoEncontradoException(string message) : AppException(message)
{
    public override int StatusCode => 404;
}
