using System.Net.Mail;
using SalesControl.Application.DTOs;
using SalesControl.Application.Exceptions;
using SalesControl.Application.Interfaces;
using SalesControl.Domain.Entities;
using SalesControl.Domain.Validation;

namespace SalesControl.Application.Services;

public class CustomerService(ICustomerRepository customers, IUnitOfWork uow) : ICustomerService
{
    public async Task<IReadOnlyList<CustomerResponse>> ListAsync(CancellationToken ct = default)
    {
        var list = await customers.ListAsync(ct);
        return list.Select(ToResponse).ToList();
    }

    public async Task<IReadOnlyList<CustomerResponse>> SearchAsync(string term, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(term))
            return await ListAsync(ct);

        var list = await customers.SearchByNameOrCpfAsync(term.Trim(), ct);
        return list.Select(ToResponse).ToList();
    }

    public async Task<CustomerResponse> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return ToResponse(await GetOrThrowAsync(id, ct));
    }

    public async Task<CustomerResponse> CreateAsync(CustomerRequest request, CancellationToken ct = default)
    {
        var customer = new Customer();
        await ApplyDataAsync(customer, request, ct);

        customer.CreatedAt = customer.UpdatedAt = DateTime.UtcNow;

        await customers.AddAsync(customer, ct);
        await uow.SaveAsync(ct);

        return ToResponse(customer);
    }

    public async Task<CustomerResponse> UpdateAsync(long id, CustomerRequest request, CancellationToken ct = default)
    {
        var customer = await GetOrThrowAsync(id, ct);
        await ApplyDataAsync(customer, request, ct);

        customer.UpdatedAt = DateTime.UtcNow;

        customers.Update(customer);
        await uow.SaveAsync(ct);

        return ToResponse(customer);
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var customer = await GetOrThrowAsync(id, ct);

        customers.Remove(customer);
        await uow.SaveAsync(ct);
    }

    private async Task<Customer> GetOrThrowAsync(long id, CancellationToken ct)
    {
        return await customers.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Cliente não encontrado.");
    }

    private async Task ApplyDataAsync(Customer customer, CustomerRequest request, CancellationToken ct)
    {
        var cpf = CpfValidator.Normalize(request.Cpf);
        if (!CpfValidator.IsValid(cpf))
            throw new ValidationException("CPF inválido.");

        var withSameCpf = await customers.GetByCpfAsync(cpf, ct);
        if (withSameCpf is not null && withSameCpf.Id != customer.Id)
            throw new ConflictException("Já existe um cliente cadastrado com este CPF.");

        var name = (request.Name ?? string.Empty).Trim();
        if (name.Length == 0)
            throw new ValidationException("O nome é obrigatório.");
        if (name.Length > 200)
            throw new ValidationException("O nome deve ter no máximo 200 caracteres.");

        if (request.BirthDate > DateOnly.FromDateTime(DateTime.Today))
            throw new ValidationException("A data de nascimento não pode ser futura.");

        var email = request.Email?.Trim();
        if (email?.Length > 200)
            throw new ValidationException("O e-mail deve ter no máximo 200 caracteres.");
        if (!string.IsNullOrEmpty(email) && !IsValidEmail(email))
            throw new ValidationException("E-mail inválido.");

        var phone = request.Phone?.Trim();
        if (phone?.Length > 20)
            throw new ValidationException("O telefone deve ter no máximo 20 caracteres.");

        customer.Cpf = cpf;
        customer.Name = name;
        customer.BirthDate = request.BirthDate;
        customer.Email = string.IsNullOrEmpty(email) ? null : email;
        customer.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone;
    }

    private static bool IsValidEmail(string email)
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

    private static CustomerResponse ToResponse(Customer c)
    {
        return new CustomerResponse(c.Id, c.Cpf, c.Name, c.BirthDate, c.Email, c.Phone);
    }
}
