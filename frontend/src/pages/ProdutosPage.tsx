import { type FormEvent, useEffect, useState } from 'react'
import { api } from '../services/api'
import type { Produto, ProdutoPayload } from '../types'
import { formatBRL } from '../utils/format'
import { Feedback, btnDanger, btnLink, btnPrimary, btnSecondary, inputCls, labelCls } from '../components/ui'

const formVazio: ProdutoPayload = { nome: '', valorUnitario: 0 }

export default function ProdutosPage() {
  const [produtos, setProdutos] = useState<Produto[]>([])
  const [form, setForm] = useState<ProdutoPayload>(formVazio)
  const [valorTexto, setValorTexto] = useState('')
  const [editandoId, setEditandoId] = useState<number | null>(null)
  const [mostrandoForm, setMostrandoForm] = useState(false)
  const [erro, setErro] = useState<string | null>(null)
  const [sucesso, setSucesso] = useState<string | null>(null)
  const [salvando, setSalvando] = useState(false)

  async function carregar() {
    try {
      setProdutos(await api.get<Produto[]>('/produtos'))
    } catch (e) {
      setErro(e instanceof Error ? e.message : 'Erro ao carregar produtos.')
    }
  }

  useEffect(() => {
    void carregar()
  }, [])

  function novo() {
    setForm(formVazio)
    setValorTexto('')
    setEditandoId(null)
    setMostrandoForm(true)
    setErro(null)
    setSucesso(null)
  }

  function editar(produto: Produto) {
    setForm({ nome: produto.nome, valorUnitario: produto.valorUnitario })
    setValorTexto(String(produto.valorUnitario).replace('.', ','))
    setEditandoId(produto.id)
    setMostrandoForm(true)
    setErro(null)
    setSucesso(null)
  }

  async function salvar(e: FormEvent) {
    e.preventDefault()
    setSalvando(true)
    setErro(null)
    setSucesso(null)

    const payload: ProdutoPayload = {
      nome: form.nome.trim(),
      valorUnitario: Number(valorTexto.replace(',', '.')) || 0,
    }

    try {
      if (editandoId === null) {
        await api.post('/produtos', payload)
        setSucesso('Produto cadastrado com sucesso.')
      } else {
        await api.put(`/produtos/${editandoId}`, payload)
        setSucesso('Produto atualizado com sucesso.')
      }
      setMostrandoForm(false)
      setForm(formVazio)
      setValorTexto('')
      setEditandoId(null)
      await carregar()
    } catch (e2) {
      setErro(e2 instanceof Error ? e2.message : 'Erro ao salvar produto.')
    } finally {
      setSalvando(false)
    }
  }

  async function remover(produto: Produto) {
    if (!window.confirm(`Remover o produto "${produto.nome}"?`)) return
    setErro(null)
    setSucesso(null)
    try {
      await api.del(`/produtos/${produto.id}`)
      setSucesso('Produto removido.')
      await carregar()
    } catch (e) {
      setErro(e instanceof Error ? e.message : 'Erro ao remover produto.')
    }
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold text-slate-900">Produtos</h1>
        <button type="button" className={btnPrimary} onClick={novo}>
          Novo produto
        </button>
      </div>

      <Feedback erro={erro} sucesso={sucesso} />

      {mostrandoForm && (
        <form
          onSubmit={salvar}
          className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm"
        >
          <h2 className="mb-4 text-sm font-semibold text-slate-900">
            {editandoId === null ? 'Novo produto' : 'Editar produto'}
          </h2>
          <div className="grid gap-4 sm:grid-cols-2">
            <div>
              <label className={labelCls}>Nome *</label>
              <input
                className={inputCls}
                value={form.nome}
                onChange={(e) => setForm({ ...form, nome: e.target.value })}
                required
              />
            </div>
            <div>
              <label className={labelCls}>Valor unitário *</label>
              <input
                className={inputCls}
                inputMode="decimal"
                placeholder="0,00"
                value={valorTexto}
                onChange={(e) =>
                  setValorTexto(e.target.value.replace(/[^\d,.-]/g, ''))
                }
                required
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
              <th className="px-4 py-3 text-right">Valor unitário</th>
              <th className="px-4 py-3 text-right">Ações</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100">
            {produtos.map((produto) => (
              <tr key={produto.id} className="hover:bg-slate-50">
                <td className="px-4 py-3 font-medium text-slate-900">{produto.nome}</td>
                <td className="px-4 py-3 text-right">{formatBRL(produto.valorUnitario)}</td>
                <td className="px-4 py-3 text-right">
                  <button type="button" className={`${btnLink} mr-3`} onClick={() => editar(produto)}>
                    Editar
                  </button>
                  <button type="button" className={btnDanger} onClick={() => remover(produto)}>
                    Remover
                  </button>
                </td>
              </tr>
            ))}
            {produtos.length === 0 && (
              <tr>
                <td colSpan={3} className="px-4 py-8 text-center text-slate-400">
                  Nenhum produto cadastrado.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  )
}
