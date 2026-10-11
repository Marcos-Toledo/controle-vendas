using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SalesControl.Application.Exceptions;

namespace SalesControl.Api.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AppException ex)
        {
            await WriteErrorAsync(context, ex.StatusCode, ex.Message);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg)
        {
            var (status, message) = pg.SqlState switch
            {
                PostgresErrorCodes.UniqueViolation =>
                    (409, "Já existe um registro cadastrado com esses dados."),
                PostgresErrorCodes.ForeignKeyViolation =>
                    (409, "Não é possível remover: este registro está em uso."),
                _ => (400, "Não foi possível salvar os dados informados.")
            };

            await WriteErrorAsync(context, status, message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro inesperado ao processar {Path}", context.Request.Path);
            await WriteErrorAsync(context, 500, "Ocorreu um erro inesperado. Tente novamente.");
        }
    }

    private static Task WriteErrorAsync(HttpContext context, int status, string message)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(new { message }));
    }
}
