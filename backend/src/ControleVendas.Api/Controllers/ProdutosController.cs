using ControleVendas.Application.DTOs;
using ControleVendas.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleVendas.Api.Controllers;

[ApiController]
[Route("api/produtos")]
public class ProdutosController(IProdutoService service) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<ProdutoResponse>> Listar(CancellationToken ct)
    {
        return service.ListarAsync(ct);
    }

    [HttpGet("{id:long}")]
    public Task<ProdutoResponse> ObterPorId(long id, CancellationToken ct)
    {
        return service.ObterPorIdAsync(id, ct);
    }

    [HttpPost]
    public async Task<ActionResult<ProdutoResponse>> Criar(
        ProdutoRequest request, CancellationToken ct)
    {
        var produto = await service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
    }

    [HttpPut("{id:long}")]
    public Task<ProdutoResponse> Atualizar(
        long id, ProdutoRequest request, CancellationToken ct)
    {
        return service.AtualizarAsync(id, request, ct);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Remover(long id, CancellationToken ct)
    {
        await service.RemoverAsync(id, ct);
        return NoContent();
    }
}
