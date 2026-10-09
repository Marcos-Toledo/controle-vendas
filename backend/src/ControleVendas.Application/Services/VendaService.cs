using ControleVendas.Application.DTOs;
using ControleVendas.Application.Exceptions;
using ControleVendas.Application.Interfaces;
using ControleVendas.Domain.Entities;

namespace ControleVendas.Application.Services;

public class VendaService(
    IVendaRepository vendas,
    IClienteRepository clientes,
    IProdutoRepository produtos,
    IUnitOfWork uow) : IVendaService
{
    public async Task<IReadOnlyList<VendaResponse>> ListarAsync(CancellationToken ct = default)
    {
        var lista = await vendas.ListarAsync(ct);
        return lista.Select(ParaResponse).ToList();
    }

    public async Task<VendaResponse> ObterPorIdAsync(long id, CancellationToken ct = default)
    {
        var venda = await vendas.ObterPorIdAsync(id, ct)
            ?? throw new NaoEncontradoException("Venda não encontrada.");

        return ParaResponse(venda);
    }

    public async Task<VendaResponse> CriarAsync(VendaRequest request, CancellationToken ct = default)
    {
        var cliente = await clientes.ObterPorIdAsync(request.ClienteId, ct)
            ?? throw new NaoEncontradoException("Cliente não encontrado.");

        if (request.Itens is not { Count: > 0 })
            throw new ValidacaoException("A venda deve conter pelo menos um produto.");

        var venda = new Venda
        {
            ClienteId = cliente.Id,
            DataVenda = request.DataVenda ?? DateOnly.FromDateTime(DateTime.Today)
        };

        foreach (var grupo in request.Itens.GroupBy(i => i.ProdutoId))
        {
            var produto = await produtos.ObterPorIdAsync(grupo.Key, ct)
                ?? throw new ValidacaoException($"Produto de código {grupo.Key} não encontrado.");

            var quantidade = grupo.Sum(i => i.Quantidade);
            if (quantidade <= 0)
                throw new ValidacaoException(
                    $"A quantidade do produto \"{produto.Nome}\" deve ser maior que zero.");

            venda.Itens.Add(new ItemVenda
            {
                ProdutoId = produto.Id,
                Quantidade = quantidade,
                PrecoUnitario = produto.ValorUnitario
            });
        }

        venda.RecalcularTotal();

        await vendas.AdicionarAsync(venda, ct);
        await uow.SalvarAsync(ct);

        var gravada = await vendas.ObterPorIdAsync(venda.Id, ct);
        return ParaResponse(gravada!);
    }

    private static VendaResponse ParaResponse(Venda v)
    {
        var itens = v.Itens
            .Select(i => new ItemVendaResponse(
                i.ProdutoId,
                i.Produto?.Nome ?? string.Empty,
                i.Quantidade,
                i.PrecoUnitario,
                i.TotalItem))
            .ToList();

        return new VendaResponse(
            v.Id,
            v.ClienteId,
            v.Cliente?.Nome ?? string.Empty,
            v.Cliente?.Cpf ?? string.Empty,
            v.DataVenda,
            v.ValorTotal,
            itens);
    }
}
