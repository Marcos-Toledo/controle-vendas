namespace ControleVendas.Application.DTOs;

public record ClienteRequest(
    string? Cpf,
    string? Nome,
    DateOnly? DataNascimento,
    string? Email,
    string? Telefone);

public record ClienteResponse(
    long Id,
    string Cpf,
    string Nome,
    DateOnly? DataNascimento,
    string? Email,
    string? Telefone);
