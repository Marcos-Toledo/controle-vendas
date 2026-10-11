namespace SalesControl.Application.DTOs;

public record ProductRequest(
    string? Name,
    decimal? UnitPrice);

public record ProductResponse(
    long Id,
    string Name,
    decimal UnitPrice);
