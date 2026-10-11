namespace SalesControl.Application.DTOs;

public record CustomerRequest(
    string? Cpf,
    string? Name,
    DateOnly? BirthDate,
    string? Email,
    string? Phone);

public record CustomerResponse(
    long Id,
    string Cpf,
    string Name,
    DateOnly? BirthDate,
    string? Email,
    string? Phone);
