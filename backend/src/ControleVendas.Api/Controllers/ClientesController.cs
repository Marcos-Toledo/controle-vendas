using ControleVendas.Application.DTOs;
using ControleVendas.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleVendas.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController(IClienteService service) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<ClienteResponse>> Listar(CancellationToken ct)
    {
        return service.ListarAsync(ct);
    }

    [HttpGet("buscar")]
    public Task<IReadOnlyList<ClienteResponse>> Buscar(
        [FromQuery] string termo, CancellationToken ct)
    {
        return service.BuscarAsync(termo, ct);
    }

    [HttpGet("{id:long}")]
    public Task<ClienteResponse> ObterPorId(long id, CancellationToken ct)
    {
        return service.ObterPorIdAsync(id, ct);
    }

    [HttpPost]
    public async Task<ActionResult<ClienteResponse>> Criar(
        ClienteRequest request, CancellationToken ct)
    {
        var cliente = await service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id:long}")]
    public Task<ClienteResponse> Atualizar(
        long id, ClienteRequest request, CancellationToken ct)
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
