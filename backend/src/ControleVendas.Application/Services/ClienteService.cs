using System.Net.Mail;
using ControleVendas.Application.DTOs;
using ControleVendas.Application.Exceptions;
using ControleVendas.Application.Interfaces;
using ControleVendas.Domain.Entities;
using ControleVendas.Domain.Validation;

namespace ControleVendas.Application.Services;

public class ClienteService(IClienteRepository clientes, IUnitOfWork uow) : IClienteService
{
    public async Task<IReadOnlyList<ClienteResponse>> ListarAsync(CancellationToken ct = default)
    {
        var lista = await clientes.ListarAsync(ct);
        return lista.Select(ParaResponse).ToList();
    }

    public async Task<IReadOnlyList<ClienteResponse>> BuscarAsync(string termo, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return await ListarAsync(ct);

        var lista = await clientes.BuscarPorNomeOuCpfAsync(termo.Trim(), ct);
        return lista.Select(ParaResponse).ToList();
    }

    public async Task<ClienteResponse> ObterPorIdAsync(long id, CancellationToken ct = default)
    {
        return ParaResponse(await ObterOuFalharAsync(id, ct));
    }

    public async Task<ClienteResponse> CriarAsync(ClienteRequest request, CancellationToken ct = default)
    {
        var cliente = new Cliente();
        await AplicarDadosAsync(cliente, request, ct);

        cliente.CriadoEm = cliente.AtualizadoEm = DateTime.UtcNow;

        await clientes.AdicionarAsync(cliente, ct);
        await uow.SalvarAsync(ct);

        return ParaResponse(cliente);
    }

    public async Task<ClienteResponse> AtualizarAsync(long id, ClienteRequest request, CancellationToken ct = default)
    {
        var cliente = await ObterOuFalharAsync(id, ct);
        await AplicarDadosAsync(cliente, request, ct);

        cliente.AtualizadoEm = DateTime.UtcNow;

        clientes.Atualizar(cliente);
        await uow.SalvarAsync(ct);

        return ParaResponse(cliente);
    }

    public async Task RemoverAsync(long id, CancellationToken ct = default)
    {
        var cliente = await ObterOuFalharAsync(id, ct);

        clientes.Remover(cliente);
        await uow.SalvarAsync(ct);
    }

    private async Task<Cliente> ObterOuFalharAsync(long id, CancellationToken ct)
    {
        return await clientes.ObterPorIdAsync(id, ct)
            ?? throw new NaoEncontradoException("Cliente não encontrado.");
    }

    private async Task AplicarDadosAsync(Cliente cliente, ClienteRequest request, CancellationToken ct)
    {
        var cpf = CpfValidator.Normalizar(request.Cpf);
        if (!CpfValidator.EhValido(cpf))
            throw new ValidacaoException("CPF inválido.");

        var comMesmoCpf = await clientes.ObterPorCpfAsync(cpf, ct);
        if (comMesmoCpf is not null && comMesmoCpf.Id != cliente.Id)
            throw new ConflitoException("Já existe um cliente cadastrado com este CPF.");

        var nome = (request.Nome ?? string.Empty).Trim();
        if (nome.Length == 0)
            throw new ValidacaoException("O nome é obrigatório.");

        var email = request.Email?.Trim();
        if (!string.IsNullOrEmpty(email) && !EmailEhValido(email))
            throw new ValidacaoException("E-mail inválido.");

        cliente.Cpf = cpf;
        cliente.Nome = nome;
        cliente.DataNascimento = request.DataNascimento;
        cliente.Email = string.IsNullOrEmpty(email) ? null : email;
        cliente.Telefone = string.IsNullOrWhiteSpace(request.Telefone) ? null : request.Telefone.Trim();
    }

    private static bool EmailEhValido(string email)
    {
        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static ClienteResponse ParaResponse(Cliente c)
    {
        return new ClienteResponse(c.Id, c.Cpf, c.Nome, c.DataNascimento, c.Email, c.Telefone);
    }
}
