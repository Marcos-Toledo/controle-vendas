import { type FormEvent, useEffect, useMemo, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { api } from '../services/api'
import type { Cliente, NovaVendaPayload, Produto } from '../types'
import { formatBRL, formatCpf, hojeISO } from '../utils/format'
import { Feedback, btnDanger, btnPrimary, btnSecondary, inputCls, labelCls } from '../components/ui'

interface ItemLinha {
  chave: number
  produtoId: number | ''
  quantidade: number
}

let proximaChave = 1
const novaLinha = (): ItemLinha => ({ chave: proximaChave++, produtoId: '', quantidade: 1 })

export default function NovaVendaPage() {
  const navigate = useNavigate()

  const [produtos, setProdutos] = useState<Produto[]>([])
  const [termo, setTermo] = useState('')
  const [sugestoes, setSugestoes] = useState<Cliente[]>([])
  const [buscando, setBuscando] = useState(false)
  const [cliente, setCliente] = useState<Cliente | null>(null)
  const [dataVenda, setDataVenda] = useState(hojeISO())
  const [itens, setItens] = useState<ItemLinha[]>([novaLinha()])
  const [erro, setErro] = useState<string | null>(null)
  const [salvando, setSalvando] = useState(false)
  const caixaBusca = useRef<HTMLDivElement>(null)

  useEffect(() => {
    api
      .get<Produto[]>('/produtos')
      .then(setProdutos)
      .catch((e) => setErro(e instanceof Error ? e.message : 'Erro ao carregar produtos.'))
  }, [])

  useEffect(() => {
    if (cliente || termo.trim().length < 2) {
      setSugestoes([])
      return
    }
    setBuscando(true)
    const timer = setTimeout(() => {
      api
        .get<Cliente[]>(`/clientes/buscar?termo=${encodeURIComponent(termo.trim())}`)
        .then(setSugestoes)
        .catch(() => setSugestoes([]))
        .finally(() => setBuscando(false))
    }, 300)
    return () => clearTimeout(timer)
  }, [termo, cliente])

  useEffect(() => {
    function clicarFora(e: MouseEvent) {
      if (!caixaBusca.current?.contains(e.target as Node)) setSugestoes([])
    }
    document.addEventListener('mousedown', clicarFora)
    return () => document.removeEventListener('mousedown', clicarFora)
  }, [])

  const produtoPorId = useMemo(
    () => new Map(produtos.map((p) => [p.id, p])),
    [produtos],
  )

  const totalGeral = useMemo(
    () =>
      itens.reduce((total, item) => {
        const produto = item.produtoId === '' ? undefined : produtoPorId.get(item.produtoId)
        return total + (produto ? produto.valorUnitario * item.quantidade : 0)
      }, 0),
    [itens, produtoPorId],
  )

  function selecionarCliente(c: Cliente) {
    setCliente(c)
    setTermo('')
    setSugestoes([])
  }

  function atualizarItem(chave: number, patch: Partial<ItemLinha>) {
    setItens((atual) =>
      atual.map((item) => (item.chave === chave ? { ...item, ...patch } : item)),
    )
  }

  function removerItem(chave: number) {
    setItens((atual) => (atual.length > 1 ? atual.filter((i) => i.chave !== chave) : atual))
  }

  async function finalizar(e: FormEvent) {
    e.preventDefault()
    setErro(null)

    if (!cliente) {
      setErro('Selecione um cliente para a venda.')
      return
    }

    const itensValidos = itens.filter((i) => i.produtoId !== '' && i.quantidade > 0)
    if (itensValidos.length === 0) {
      setErro('Adicione pelo menos um produto com quantidade válida.')
      return
    }

    const payload: NovaVendaPayload = {
      clienteId: cliente.id,
      dataVenda,
      itens: itensValidos.map((i) => ({
        produtoId: i.produtoId as number,
        quantidade: i.quantidade,
      })),
    }

    setSalvando(true)
    try {
      await api.post('/vendas', payload)
      navigate('/vendas')
    } catch (e2) {
      setErro(e2 instanceof Error ? e2.message : 'Erro ao registrar a venda.')
    } finally {
      setSalvando(false)
    }
  }

  return (
    <form onSubmit={finalizar} className="space-y-4">
      <h1 className="text-xl font-semibold text-slate-900">Nova Venda</h1>

      <Feedback erro={erro} />

      <section className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
        <h2 className="mb-3 text-sm font-semibold text-slate-900">Cliente</h2>
        {cliente ? (
          <div className="flex items-center justify-between rounded-lg border border-indigo-200 bg-indigo-50 px-4 py-3">
            <div>
              <div className="font-medium text-slate-900">{cliente.nome}</div>
              <div className="text-xs text-slate-500">CPF {formatCpf(cliente.cpf)}</div>
            </div>
            <button
              type="button"
              className={btnDanger}
              onClick={() => setCliente(null)}
            >
              Trocar
            </button>
          </div>
        ) : (
          <div className="relative" ref={caixaBusca}>
            <label className={labelCls}>Buscar por CPF ou nome *</label>
            <input
              className={inputCls}
              value={termo}
              onChange={(e) => setTermo(e.target.value)}
              placeholder="Digite ao menos 2 caracteres…"
            />
            {(sugestoes.length > 0 || buscando) && (
              <ul className="absolute z-10 mt-1 max-h-60 w-full overflow-auto rounded-lg border border-slate-200 bg-white py-1 shadow-lg">
                {buscando && sugestoes.length === 0 && (
                  <li className="px-4 py-2 text-sm text-slate-400">Buscando…</li>
                )}
                {sugestoes.map((c) => (
                  <li key={c.id}>
                    <button
                      type="button"
                      className="w-full px-4 py-2 text-left text-sm hover:bg-indigo-50"
                      onClick={() => selecionarCliente(c)}
                    >
                      <span className="font-medium text-slate-900">{c.nome}</span>
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
            value={dataVenda}
            onChange={(e) => setDataVenda(e.target.value)}
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
            onClick={() => setItens((a) => [...a, novaLinha()])}
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
            {itens.map((item) => {
              const produto =
                item.produtoId === '' ? undefined : produtoPorId.get(item.produtoId)
              const totalLinha = produto ? produto.valorUnitario * item.quantidade : 0
              return (
                <tr key={item.chave}>
                  <td className="py-1 pr-3">
                    <select
                      className={inputCls}
                      value={item.produtoId}
                      onChange={(e) =>
                        atualizarItem(item.chave, {
                          produtoId: e.target.value === '' ? '' : Number(e.target.value),
                        })
                      }
                    >
                      <option value="">Selecione…</option>
                      {produtos.map((p) => (
                        <option key={p.id} value={p.id}>
                          {p.nome}
                        </option>
                      ))}
                    </select>
                  </td>
                  <td className="py-1 pr-3">
                    <input
                      type="number"
                      min={1}
                      className={`${inputCls} w-24`}
                      value={item.quantidade}
                      onChange={(e) =>
                        atualizarItem(item.chave, {
                          quantidade: Math.max(0, Number(e.target.value)),
                        })
                      }
                    />
                  </td>
                  <td className="py-1 pr-3 text-right pt-3">
                    {produto ? formatBRL(produto.valorUnitario) : '—'}
                  </td>
                  <td className="py-1 pr-3 text-right font-medium pt-3">
                    {produto ? formatBRL(totalLinha) : '—'}
                  </td>
                  <td className="py-1 text-right pt-2">
                    <button
                      type="button"
                      className={btnDanger}
                      onClick={() => removerItem(item.chave)}
                      disabled={itens.length === 1}
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
          <div className="text-2xl font-bold text-slate-900">{formatBRL(totalGeral)}</div>
        </div>
        <button type="submit" className={btnPrimary} disabled={salvando}>
          {salvando ? 'Registrando…' : 'Finalizar venda'}
        </button>
      </div>
    </form>
  )
}
