namespace ControleVendas.Application.DTOs;

public record ItemVendaRequest(
    long ProdutoId,
    int Quantidade);

public record VendaRequest(
    long ClienteId,
    DateOnly? DataVenda,
    List<ItemVendaRequest>? Itens);

public record ItemVendaResponse(
    long ProdutoId,
    string ProdutoNome,
    int Quantidade,
    decimal PrecoUnitario,
    decimal TotalItem);

public record VendaResponse(
    long Id,
    long ClienteId,
    string ClienteNome,
    string ClienteCpf,
    DateOnly DataVenda,
    decimal ValorTotal,
    List<ItemVendaResponse> Itens);
