using ControleVendas.Application.DTOs;
using ControleVendas.Application.Exceptions;
using ControleVendas.Application.Interfaces;
using ControleVendas.Domain.Entities;

namespace ControleVendas.Application.Services;

public class ProdutoService(IProdutoRepository produtos, IUnitOfWork uow) : IProdutoService
{
    public async Task<IReadOnlyList<ProdutoResponse>> ListarAsync(CancellationToken ct = default)
    {
        var lista = await produtos.ListarAsync(ct);
        return lista.Select(ParaResponse).ToList();
    }

    public async Task<ProdutoResponse> ObterPorIdAsync(long id, CancellationToken ct = default)
    {
        return ParaResponse(await ObterOuFalharAsync(id, ct));
    }

    public async Task<ProdutoResponse> CriarAsync(ProdutoRequest request, CancellationToken ct = default)
    {
        var produto = new Produto();
        await AplicarDadosAsync(produto, request, ct);

        produto.CriadoEm = produto.AtualizadoEm = DateTime.UtcNow;

        await produtos.AdicionarAsync(produto, ct);
        await uow.SalvarAsync(ct);

        return ParaResponse(produto);
    }

    public async Task<ProdutoResponse> AtualizarAsync(long id, ProdutoRequest request, CancellationToken ct = default)
    {
        var produto = await ObterOuFalharAsync(id, ct);
        await AplicarDadosAsync(produto, request, ct);

        produto.AtualizadoEm = DateTime.UtcNow;

        produtos.Atualizar(produto);
        await uow.SalvarAsync(ct);

        return ParaResponse(produto);
    }

    public async Task RemoverAsync(long id, CancellationToken ct = default)
    {
        var produto = await ObterOuFalharAsync(id, ct);

        produtos.Remover(produto);
        await uow.SalvarAsync(ct);
    }

    private async Task<Produto> ObterOuFalharAsync(long id, CancellationToken ct)
    {
        return await produtos.ObterPorIdAsync(id, ct)
            ?? throw new NaoEncontradoException("Produto não encontrado.");
    }

    private async Task AplicarDadosAsync(Produto produto, ProdutoRequest request, CancellationToken ct)
    {
        var nome = (request.Nome ?? string.Empty).Trim();
        if (nome.Length == 0)
            throw new ValidacaoException("O nome é obrigatório.");

        var comMesmoNome = await produtos.ObterPorNomeAsync(nome, ct);
        if (comMesmoNome is not null && comMesmoNome.Id != produto.Id)
            throw new ConflitoException("Já existe um produto cadastrado com este nome.");

        if (request.ValorUnitario < 0)
            throw new ValidacaoException("O valor unitário deve ser maior ou igual a zero.");

        produto.Nome = nome;
        produto.ValorUnitario = request.ValorUnitario;
    }

    private static ProdutoResponse ParaResponse(Produto p)
    {
        return new ProdutoResponse(p.Id, p.Nome, p.ValorUnitario);
    }
}
