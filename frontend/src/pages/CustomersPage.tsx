import { type FormEvent, useEffect, useState } from 'react'
import { api } from '../services/api'
import type { Customer, CustomerPayload } from '../types'
import { formatCpf, formatDate, formatPhone, maskCpf, maskPhone } from '../utils/format'
import { Feedback, btnDanger, btnLink, btnPrimary, btnSecondary, inputCls, labelCls } from '../components/ui'

const emptyForm: CustomerPayload = {
  cpf: '',
  name: '',
  birthDate: null,
  email: null,
  phone: null,
}

export default function CustomersPage() {
  const [customers, setCustomers] = useState<Customer[]>([])
  const [form, setForm] = useState<CustomerPayload>(emptyForm)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [showForm, setShowForm] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)

  async function load() {
    try {
      setCustomers(await api.get<Customer[]>('/customers'))
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Erro ao carregar clientes.')
    }
  }

  useEffect(() => {
    void load()
  }, [])

  function openNew() {
    setForm(emptyForm)
    setEditingId(null)
    setShowForm(true)
    setError(null)
    setSuccess(null)
  }

  function edit(customer: Customer) {
    setForm({
      cpf: formatCpf(customer.cpf),
      name: customer.name,
      birthDate: customer.birthDate,
      email: customer.email,
      phone: customer.phone ? formatPhone(customer.phone) : null,
    })
    setEditingId(customer.id)
    setShowForm(true)
    setError(null)
    setSuccess(null)
  }

  async function save(e: FormEvent) {
    e.preventDefault()
    setSaving(true)
    setError(null)
    setSuccess(null)

    const payload: CustomerPayload = {
      cpf: form.cpf.replace(/\D/g, ''),
      name: form.name.trim(),
      birthDate: form.birthDate || null,
      email: form.email?.trim() || null,
      phone: form.phone?.replace(/\D/g, '') || null,
    }

    try {
      if (editingId === null) {
        await api.post('/customers', payload)
        setSuccess('Cliente cadastrado com sucesso.')
      } else {
        await api.put(`/customers/${editingId}`, payload)
        setSuccess('Cliente atualizado com sucesso.')
      }
      setShowForm(false)
      setForm(emptyForm)
      setEditingId(null)
      await load()
    } catch (e2) {
      setError(e2 instanceof Error ? e2.message : 'Erro ao salvar cliente.')
    } finally {
      setSaving(false)
    }
  }

  async function remove(customer: Customer) {
    if (!window.confirm(`Remover o cliente "${customer.name}"?`)) return
    setError(null)
    setSuccess(null)
    try {
      await api.del(`/customers/${customer.id}`)
      setSuccess('Cliente removido.')
      await load()
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Erro ao remover cliente.')
    }
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold text-slate-900">Clientes</h1>
        <button type="button" className={btnPrimary} onClick={openNew}>
          Novo cliente
        </button>
      </div>

      <Feedback error={error} success={success} />

      {showForm && (
        <form
          onSubmit={save}
          className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm"
        >
          <h2 className="mb-4 text-sm font-semibold text-slate-900">
            {editingId === null ? 'Novo cliente' : 'Editar cliente'}
          </h2>
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            <div>
              <label className={labelCls}>CPF *</label>
              <input
                className={inputCls}
                value={form.cpf}
                onChange={(e) => setForm({ ...form, cpf: maskCpf(e.target.value) })}
                placeholder="000.000.000-00"
                required
              />
            </div>
            <div className="sm:col-span-2">
              <label className={labelCls}>Nome *</label>
              <input
                className={inputCls}
                value={form.name}
                onChange={(e) => setForm({ ...form, name: e.target.value })}
                required
              />
            </div>
            <div>
              <label className={labelCls}>Data de nascimento</label>
              <input
                type="date"
                className={inputCls}
                value={form.birthDate ?? ''}
                onChange={(e) =>
                  setForm({ ...form, birthDate: e.target.value || null })
                }
              />
            </div>
            <div>
              <label className={labelCls}>E-mail</label>
              <input
                type="email"
                className={inputCls}
                value={form.email ?? ''}
                onChange={(e) => setForm({ ...form, email: e.target.value })}
              />
            </div>
            <div>
              <label className={labelCls}>Telefone</label>
              <input
                type="tel"
                className={inputCls}
                value={form.phone ?? ''}
                onChange={(e) => setForm({ ...form, phone: maskPhone(e.target.value) })}
                placeholder="(11) 98765-4321"
              />
            </div>
          </div>
          <div className="mt-4 flex gap-2">
            <button type="submit" className={btnPrimary} disabled={saving}>
              {saving ? 'Salvando…' : 'Salvar'}
            </button>
            <button
              type="button"
              className={btnSecondary}
              onClick={() => setShowForm(false)}
            >
              Cancelar
            </button>
          </div>
        </form>
      )}

      <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
        <table className="w-full text-left text-sm">
          <thead className="border-b border-slate-200 bg-slate-50 text-xs uppercase text-slate-500">
            <tr>
              <th className="px-4 py-3">Nome</th>
              <th className="px-4 py-3">CPF</th>
              <th className="px-4 py-3">Nascimento</th>
              <th className="px-4 py-3">E-mail</th>
              <th className="px-4 py-3">Telefone</th>
              <th className="px-4 py-3 text-right">Ações</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100">
            {customers.map((customer) => (
              <tr key={customer.id} className="hover:bg-slate-50">
                <td className="px-4 py-3 font-medium text-slate-900">{customer.name}</td>
                <td className="px-4 py-3">{formatCpf(customer.cpf)}</td>
                <td className="px-4 py-3">
                  {customer.birthDate ? formatDate(customer.birthDate) : '—'}
                </td>
                <td className="px-4 py-3">{customer.email ?? '—'}</td>
                <td className="px-4 py-3">
                  {customer.phone ? formatPhone(customer.phone) : '—'}
                </td>
                <td className="px-4 py-3 text-right">
                  <button type="button" className={`${btnLink} mr-3`} onClick={() => edit(customer)}>
                    Editar
                  </button>
                  <button type="button" className={btnDanger} onClick={() => remove(customer)}>
                    Remover
                  </button>
                </td>
              </tr>
            ))}
            {customers.length === 0 && (
              <tr>
                <td colSpan={6} className="px-4 py-8 text-center text-slate-400">
                  Nenhum cliente cadastrado.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  )
}
