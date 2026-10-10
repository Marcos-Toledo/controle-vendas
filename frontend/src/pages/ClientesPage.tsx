import { type FormEvent, useEffect, useState } from 'react'
import { api } from '../services/api'
import type { Cliente, ClientePayload } from '../types'
import { formatCpf, formatDate, formatTelefone, maskCpf, maskTelefone } from '../utils/format'
import { Feedback, btnDanger, btnLink, btnPrimary, btnSecondary, inputCls, labelCls } from '../components/ui'

const formVazio: ClientePayload = {
  cpf: '',
  nome: '',
  dataNascimento: null,
  email: null,
  telefone: null,
}

export default function ClientesPage() {
  const [clientes, setClientes] = useState<Cliente[]>([])
  const [form, setForm] = useState<ClientePayload>(formVazio)
  const [editandoId, setEditandoId] = useState<number | null>(null)
  const [mostrandoForm, setMostrandoForm] = useState(false)
  const [erro, setErro] = useState<string | null>(null)
  const [sucesso, setSucesso] = useState<string | null>(null)
  const [salvando, setSalvando] = useState(false)

  async function carregar() {
    try {
      setClientes(await api.get<Cliente[]>('/clientes'))
    } catch (e) {
      setErro(e instanceof Error ? e.message : 'Erro ao carregar clientes.')
    }
  }

  useEffect(() => {
    void carregar()
  }, [])

  function novo() {
    setForm(formVazio)
    setEditandoId(null)
    setMostrandoForm(true)
    setErro(null)
    setSucesso(null)
  }

  function editar(cliente: Cliente) {
    setForm({
      cpf: formatCpf(cliente.cpf),
      nome: cliente.nome,
      dataNascimento: cliente.dataNascimento,
      email: cliente.email,
      telefone: cliente.telefone ? formatTelefone(cliente.telefone) : null,
    })
    setEditandoId(cliente.id)
    setMostrandoForm(true)
    setErro(null)
    setSucesso(null)
  }

  async function salvar(e: FormEvent) {
    e.preventDefault()
    setSalvando(true)
    setErro(null)
    setSucesso(null)

    const payload: ClientePayload = {
      cpf: form.cpf.replace(/\D/g, ''),
      nome: form.nome.trim(),
      dataNascimento: form.dataNascimento || null,
      email: form.email?.trim() || null,
      telefone: form.telefone?.replace(/\D/g, '') || null,
    }

    try {
      if (editandoId === null) {
        await api.post('/clientes', payload)
        setSucesso('Cliente cadastrado com sucesso.')
      } else {
        await api.put(`/clientes/${editandoId}`, payload)
        setSucesso('Cliente atualizado com sucesso.')
      }
      setMostrandoForm(false)
      setForm(formVazio)
      setEditandoId(null)
      await carregar()
    } catch (e2) {
      setErro(e2 instanceof Error ? e2.message : 'Erro ao salvar cliente.')
    } finally {
      setSalvando(false)
    }
  }

  async function remover(cliente: Cliente) {
    if (!window.confirm(`Remover o cliente "${cliente.nome}"?`)) return
    setErro(null)
    setSucesso(null)
    try {
      await api.del(`/clientes/${cliente.id}`)
      setSucesso('Cliente removido.')
      await carregar()
    } catch (e) {
      setErro(e instanceof Error ? e.message : 'Erro ao remover cliente.')
    }
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold text-slate-900">Clientes</h1>
        <button type="button" className={btnPrimary} onClick={novo}>
          Novo cliente
        </button>
      </div>

      <Feedback erro={erro} sucesso={sucesso} />

      {mostrandoForm && (
        <form
          onSubmit={salvar}
          className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm"
        >
          <h2 className="mb-4 text-sm font-semibold text-slate-900">
            {editandoId === null ? 'Novo cliente' : 'Editar cliente'}
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
                value={form.nome}
                onChange={(e) => setForm({ ...form, nome: e.target.value })}
                required
              />
            </div>
            <div>
              <label className={labelCls}>Data de nascimento</label>
              <input
                type="date"
                className={inputCls}
                value={form.dataNascimento ?? ''}
                onChange={(e) =>
                  setForm({ ...form, dataNascimento: e.target.value || null })
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
                value={form.telefone ?? ''}
                onChange={(e) => setForm({ ...form, telefone: maskTelefone(e.target.value) })}
                placeholder="(11) 98765-4321"
              />
            </div>
          </div>
          <div className="mt-4 flex gap-2">
            <button type="submit" className={btnPrimary} disabled={salvando}>
              {salvando ? 'Salvando…' : 'Salvar'}
            </button>
            <button
              type="button"
              className={btnSecondary}
              onClick={() => setMostrandoForm(false)}
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
            {clientes.map((cliente) => (
              <tr key={cliente.id} className="hover:bg-slate-50">
                <td className="px-4 py-3 font-medium text-slate-900">{cliente.nome}</td>
                <td className="px-4 py-3">{formatCpf(cliente.cpf)}</td>
                <td className="px-4 py-3">
                  {cliente.dataNascimento ? formatDate(cliente.dataNascimento) : '—'}
                </td>
                <td className="px-4 py-3">{cliente.email ?? '—'}</td>
                <td className="px-4 py-3">
                  {cliente.telefone ? formatTelefone(cliente.telefone) : '—'}
                </td>
                <td className="px-4 py-3 text-right">
                  <button type="button" className={`${btnLink} mr-3`} onClick={() => editar(cliente)}>
                    Editar
                  </button>
                  <button type="button" className={btnDanger} onClick={() => remover(cliente)}>
                    Remover
                  </button>
                </td>
              </tr>
            ))}
            {clientes.length === 0 && (
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
