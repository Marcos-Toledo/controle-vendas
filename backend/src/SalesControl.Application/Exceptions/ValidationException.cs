namespace SalesControl.Application.Exceptions;

public class ValidationException(string message) : AppException(message)
{
    public override int StatusCode => 400;
}
