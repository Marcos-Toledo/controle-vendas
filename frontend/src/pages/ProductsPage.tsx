import { type FormEvent, useEffect, useState } from 'react'
import { api } from '../services/api'
import type { Product, ProductPayload } from '../types'
import { formatBRL, maskCurrency, parseCurrency } from '../utils/format'
import { Feedback, btnDanger, btnLink, btnPrimary, btnSecondary, inputCls, labelCls } from '../components/ui'

const emptyForm: ProductPayload = { name: '', unitPrice: 0 }

export default function ProductsPage() {
  const [products, setProducts] = useState<Product[]>([])
  const [form, setForm] = useState<ProductPayload>(emptyForm)
  const [priceText, setPriceText] = useState('')
  const [editingId, setEditingId] = useState<number | null>(null)
  const [showForm, setShowForm] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)

  async function load() {
    try {
      setProducts(await api.get<Product[]>('/products'))
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Erro ao carregar produtos.')
    }
  }

  useEffect(() => {
    void load()
  }, [])

  function openNew() {
    setForm(emptyForm)
    setPriceText('')
    setEditingId(null)
    setShowForm(true)
    setError(null)
    setSuccess(null)
  }

  function edit(product: Product) {
    setForm({ name: product.name, unitPrice: product.unitPrice })
    setPriceText(maskCurrency(String(Math.round(product.unitPrice * 100))))
    setEditingId(product.id)
    setShowForm(true)
    setError(null)
    setSuccess(null)
  }

  async function save(e: FormEvent) {
    e.preventDefault()
    setSaving(true)
    setError(null)
    setSuccess(null)

    const payload: ProductPayload = {
      name: form.name.trim(),
      unitPrice: parseCurrency(priceText),
    }

    try {
      if (editingId === null) {
        await api.post('/products', payload)
        setSuccess('Produto cadastrado com sucesso.')
      } else {
        await api.put(`/products/${editingId}`, payload)
        setSuccess('Produto atualizado com sucesso.')
      }
      setShowForm(false)
      setForm(emptyForm)
      setPriceText('')
      setEditingId(null)
      await load()
    } catch (e2) {
      setError(e2 instanceof Error ? e2.message : 'Erro ao salvar produto.')
    } finally {
      setSaving(false)
    }
  }

  async function remove(product: Product) {
    if (!window.confirm(`Remover o produto "${product.name}"?`)) return
    setError(null)
    setSuccess(null)
    try {
      await api.del(`/products/${product.id}`)
      setSuccess('Produto removido.')
      await load()
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Erro ao remover produto.')
    }
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold text-slate-900">Produtos</h1>
        <button type="button" className={btnPrimary} onClick={openNew}>
          Novo produto
        </button>
      </div>

      <Feedback error={error} success={success} />

      {showForm && (
        <form
          onSubmit={save}
          className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm"
        >
          <h2 className="mb-4 text-sm font-semibold text-slate-900">
            {editingId === null ? 'Novo produto' : 'Editar produto'}
          </h2>
          <div className="grid gap-4 sm:grid-cols-2">
            <div>
              <label className={labelCls}>Nome *</label>
              <input
                className={inputCls}
                value={form.name}
                onChange={(e) => setForm({ ...form, name: e.target.value })}
                required
              />
            </div>
            <div>
              <label className={labelCls}>Valor unitário *</label>
              <input
                className={inputCls}
                inputMode="numeric"
                placeholder="0,00"
                value={priceText}
                onChange={(e) => setPriceText(maskCurrency(e.target.value))}
                required
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
              <th className="px-4 py-3 text-right">Valor unitário</th>
              <th className="px-4 py-3 text-right">Ações</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100">
            {products.map((product) => (
              <tr key={product.id} className="hover:bg-slate-50">
                <td className="px-4 py-3 font-medium text-slate-900">{product.name}</td>
                <td className="px-4 py-3 text-right">{formatBRL(product.unitPrice)}</td>
                <td className="px-4 py-3 text-right">
                  <button type="button" className={`${btnLink} mr-3`} onClick={() => edit(product)}>
                    Editar
                  </button>
                  <button type="button" className={btnDanger} onClick={() => remove(product)}>
                    Remover
                  </button>
                </td>
              </tr>
            ))}
            {products.length === 0 && (
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
