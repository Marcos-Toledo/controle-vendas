namespace SalesControl.Application.DTOs;

public record SaleItemRequest(
    long ProductId,
    int Quantity);

public record SaleRequest(
    long CustomerId,
    DateOnly? SaleDate,
    List<SaleItemRequest>? Items);

public record SaleItemResponse(
    long ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal ItemTotal);

public record SaleResponse(
    long Id,
    long CustomerId,
    string CustomerName,
    string CustomerCpf,
    DateOnly SaleDate,
    decimal TotalAmount,
    List<SaleItemResponse> Items);
