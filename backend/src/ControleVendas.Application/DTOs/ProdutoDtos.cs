namespace ControleVendas.Application.DTOs;

public record ProdutoRequest(
    string? Nome,
    decimal ValorUnitario);

public record ProdutoResponse(
    long Id,
    string Nome,
    decimal ValorUnitario);
