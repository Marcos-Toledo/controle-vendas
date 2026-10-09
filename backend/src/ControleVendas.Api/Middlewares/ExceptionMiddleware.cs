using System.Text.Json;
using ControleVendas.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ControleVendas.Api.Middlewares;

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
            await EscreverErroAsync(context, ex.StatusCode, ex.Message);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg)
        {
            var (status, mensagem) = pg.SqlState switch
            {
                PostgresErrorCodes.UniqueViolation =>
                    (409, "Já existe um registro cadastrado com esses dados."),
                PostgresErrorCodes.ForeignKeyViolation =>
                    (409, "Não é possível remover: este registro está em uso."),
                _ => (400, "Não foi possível salvar os dados informados.")
            };

            await EscreverErroAsync(context, status, mensagem);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro inesperado ao processar {Path}", context.Request.Path);
            await EscreverErroAsync(context, 500, "Ocorreu um erro inesperado. Tente novamente.");
        }
    }

    private static Task EscreverErroAsync(HttpContext context, int status, string mensagem)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(new { mensagem }));
    }
}
