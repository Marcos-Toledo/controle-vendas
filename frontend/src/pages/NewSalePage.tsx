import { type FormEvent, useEffect, useMemo, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { api } from '../services/api'
import type { Customer, NewSalePayload, Product } from '../types'
import { formatBRL, formatCpf, todayISO } from '../utils/format'
import { Feedback, btnDanger, btnPrimary, btnSecondary, inputCls, labelCls } from '../components/ui'

interface ItemRow {
  key: number
  productId: number | ''
  quantity: number
}

let nextKey = 1
const newRow = (): ItemRow => ({ key: nextKey++, productId: '', quantity: 1 })

export default function NewSalePage() {
  const navigate = useNavigate()

  const [products, setProducts] = useState<Product[]>([])
  const [term, setTerm] = useState('')
  const [suggestions, setSuggestions] = useState<Customer[]>([])
  const [searching, setSearching] = useState(false)
  const [customer, setCustomer] = useState<Customer | null>(null)
  const [saleDate, setSaleDate] = useState(todayISO())
  const [items, setItems] = useState<ItemRow[]>([newRow()])
  const [error, setError] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)
  const searchBox = useRef<HTMLDivElement>(null)

  useEffect(() => {
    api
      .get<Product[]>('/products')
      .then(setProducts)
      .catch((e) => setError(e instanceof Error ? e.message : 'Erro ao carregar produtos.'))
  }, [])

  useEffect(() => {
    if (customer || term.trim().length < 2) {
      setSuggestions([])
      return
    }
    setSearching(true)
    const timer = setTimeout(() => {
      api
        .get<Customer[]>(`/customers/search?term=${encodeURIComponent(term.trim())}`)
        .then(setSuggestions)
        .catch(() => setSuggestions([]))
        .finally(() => setSearching(false))
    }, 300)
    return () => clearTimeout(timer)
  }, [term, customer])

  useEffect(() => {
    function onClickOutside(e: MouseEvent) {
      if (!searchBox.current?.contains(e.target as Node)) setSuggestions([])
    }
    document.addEventListener('mousedown', onClickOutside)
    return () => document.removeEventListener('mousedown', onClickOutside)
  }, [])

  const productById = useMemo(
    () => new Map(products.map((p) => [p.id, p])),
    [products],
  )

  const grandTotal = useMemo(
    () =>
      items.reduce((total, item) => {
        const product = item.productId === '' ? undefined : productById.get(item.productId)
        return total + (product ? product.unitPrice * item.quantity : 0)
      }, 0),
    [items, productById],
  )

  function selectCustomer(c: Customer) {
    setCustomer(c)
    setTerm('')
    setSuggestions([])
  }

  function updateItem(key: number, patch: Partial<ItemRow>) {
    setItems((current) =>
      current.map((item) => (item.key === key ? { ...item, ...patch } : item)),
    )
  }

  function removeItem(key: number) {
    setItems((current) => (current.length > 1 ? current.filter((i) => i.key !== key) : current))
  }

  async function submit(e: FormEvent) {
    e.preventDefault()
    setError(null)

    if (!customer) {
      setError('Selecione um cliente para a venda.')
      return
    }

    const validItems = items.filter((i) => i.productId !== '' && i.quantity > 0)
    if (validItems.length === 0) {
      setError('Adicione pelo menos um produto com quantidade válida.')
      return
    }

    const payload: NewSalePayload = {
      customerId: customer.id,
      saleDate,
      items: validItems.map((i) => ({
        productId: i.productId as number,
        quantity: i.quantity,
      })),
    }

    setSaving(true)
    try {
      await api.post('/sales', payload)
      navigate('/sales')
    } catch (e2) {
      setError(e2 instanceof Error ? e2.message : 'Erro ao registrar a venda.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <form onSubmit={submit} className="space-y-4">
      <h1 className="text-xl font-semibold text-slate-900">Nova Venda</h1>

      <Feedback error={error} />

      <section className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
        <h2 className="mb-3 text-sm font-semibold text-slate-900">Cliente</h2>
        {customer ? (
          <div className="flex items-center justify-between rounded-lg border border-indigo-200 bg-indigo-50 px-4 py-3">
            <div>
              <div className="font-medium text-slate-900">{customer.name}</div>
              <div className="text-xs text-slate-500">CPF {formatCpf(customer.cpf)}</div>
            </div>
            <button
              type="button"
              className={btnDanger}
              onClick={() => setCustomer(null)}
            >
              Trocar
            </button>
          </div>
        ) : (
          <div className="relative" ref={searchBox}>
            <label className={labelCls}>Buscar por CPF ou nome *</label>
            <input
              className={inputCls}
              value={term}
              onChange={(e) => setTerm(e.target.value)}
              placeholder="Digite ao menos 2 caracteres…"
            />
            {(suggestions.length > 0 || searching) && (
              <ul className="absolute z-10 mt-1 max-h-60 w-full overflow-auto rounded-lg border border-slate-200 bg-white py-1 shadow-lg">
                {searching && suggestions.length === 0 && (
                  <li className="px-4 py-2 text-sm text-slate-400">Buscando…</li>
                )}
                {suggestions.map((c) => (
                  <li key={c.id}>
                    <button
                      type="button"
                      className="w-full px-4 py-2 text-left text-sm hover:bg-indigo-50"
                      onClick={() => selectCustomer(c)}
                    >
                      <span className="font-medium text-slate-900">{c.name}</span>
                      <span className="ml-2 text-xs text-slate-500">{formatCpf(c.cpf)}</span>
                    </button>
                  </li>
                ))}
              </ul>
            )}
          </div>
        )}

        <div className="mt-4 w-48">
          <label className={labelCls}>Data da venda</label>
          <input
            type="date"
            className={inputCls}
            value={saleDate}
            onChange={(e) => setSaleDate(e.target.value)}
            required
          />
        </div>
      </section>

      <section className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
        <div className="mb-3 flex items-center justify-between">
          <h2 className="text-sm font-semibold text-slate-900">Produtos</h2>
          <button
            type="button"
            className={btnSecondary}
            onClick={() => setItems((a) => [...a, newRow()])}
          >
            + Adicionar produto
          </button>
        </div>

        <table className="w-full text-left text-sm">
          <thead className="text-xs uppercase text-slate-500">
            <tr>
              <th className="pb-2 pr-3 w-[45%]">Produto</th>
              <th className="pb-2 pr-3">Qtd.</th>
              <th className="pb-2 pr-3 text-right">Valor unit.</th>
              <th className="pb-2 pr-3 text-right">Total</th>
              <th className="pb-2" />
            </tr>
          </thead>
          <tbody className="align-top">
            {items.map((item) => {
              const product =
                item.productId === '' ? undefined : productById.get(item.productId)
              const rowTotal = product ? product.unitPrice * item.quantity : 0
              return (
                <tr key={item.key}>
                  <td className="py-1 pr-3">
                    <select
                      className={inputCls}
                      value={item.productId}
                      onChange={(e) =>
                        updateItem(item.key, {
                          productId: e.target.value === '' ? '' : Number(e.target.value),
                        })
                      }
                    >
                      <option value="">Selecione…</option>
                      {products.map((p) => (
                        <option key={p.id} value={p.id}>
                          {p.name}
                        </option>
                      ))}
                    </select>
                  </td>
                  <td className="py-1 pr-3">
                    <input
                      type="number"
                      min={1}
                      className={`${inputCls} w-24`}
                      value={item.quantity}
                      onChange={(e) =>
                        updateItem(item.key, {
                          quantity: Math.max(0, Number(e.target.value)),
                        })
                      }
                    />
                  </td>
                  <td className="py-1 pr-3 text-right pt-3">
                    {product ? formatBRL(product.unitPrice) : '—'}
                  </td>
                  <td className="py-1 pr-3 text-right font-medium pt-3">
                    {product ? formatBRL(rowTotal) : '—'}
                  </td>
                  <td className="py-1 text-right pt-2">
                    <button
                      type="button"
                      className={btnDanger}
                      onClick={() => removeItem(item.key)}
                      disabled={items.length === 1}
                    >
                      ✕
                    </button>
                  </td>
                </tr>
              )
            })}
          </tbody>
        </table>
      </section>

      <div className="flex items-center justify-between rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
        <div className="text-sm text-slate-500">
          Total da venda
          <div className="text-2xl font-bold text-slate-900">{formatBRL(grandTotal)}</div>
        </div>
        <button type="submit" className={btnPrimary} disabled={saving}>
          {saving ? 'Registrando…' : 'Finalizar venda'}
        </button>
      </div>
    </form>
  )
}
