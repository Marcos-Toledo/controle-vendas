import { Fragment, useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../services/api'
import type { Venda } from '../types'
import { formatBRL, formatCpf, formatDate } from '../utils/format'
import { Feedback, btnLink } from '../components/ui'

export default function VendasPage() {
  const [vendas, setVendas] = useState<Venda[]>([])
  const [expandida, setExpandida] = useState<number | null>(null)
  const [erro, setErro] = useState<string | null>(null)

  useEffect(() => {
    api
      .get<Venda[]>('/vendas')
      .then(setVendas)
      .catch((e) => setErro(e instanceof Error ? e.message : 'Erro ao carregar vendas.'))
  }, [])

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold text-slate-900">Vendas</h1>
        <Link to="/vendas/nova" className={btnLink}>
          + Nova venda
        </Link>
      </div>

      <Feedback erro={erro} />

      <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
        <table className="w-full text-left text-sm">
          <thead className="border-b border-slate-200 bg-slate-50 text-xs uppercase text-slate-500">
            <tr>
              <th className="px-4 py-3">Nº</th>
              <th className="px-4 py-3">Data</th>
              <th className="px-4 py-3">Cliente</th>
              <th className="px-4 py-3 text-center">Itens</th>
              <th className="px-4 py-3 text-right">Total</th>
              <th className="px-4 py-3" />
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100">
            {vendas.map((venda) => (
              <Fragment key={venda.id}>
                <tr className="hover:bg-slate-50">
                  <td className="px-4 py-3 font-medium text-slate-900">#{venda.id}</td>
                  <td className="px-4 py-3">{formatDate(venda.dataVenda)}</td>
                  <td className="px-4 py-3">
                    <div className="font-medium text-slate-900">{venda.clienteNome}</div>
                    <div className="text-xs text-slate-500">{formatCpf(venda.clienteCpf)}</div>
                  </td>
                  <td className="px-4 py-3 text-center">{venda.itens.length}</td>
                  <td className="px-4 py-3 text-right font-semibold text-slate-900">
                    {formatBRL(venda.valorTotal)}
                  </td>
                  <td className="px-4 py-3 text-right">
                    <button
                      type="button"
                      className={btnLink}
                      onClick={() =>
                        setExpandida(expandida === venda.id ? null : venda.id)
                      }
                    >
                      {expandida === venda.id ? 'Ocultar' : 'Detalhes'}
                    </button>
                  </td>
                </tr>
                {expandida === venda.id && (
                  <tr className="bg-slate-50">
                    <td colSpan={6} className="px-4 py-3">
                      <table className="w-full text-left text-xs">
                        <thead className="text-slate-500">
                          <tr>
                            <th className="py-1 pr-4">Produto</th>
                            <th className="py-1 pr-4 text-right">Qtd.</th>
                            <th className="py-1 pr-4 text-right">Preço unit.</th>
                            <th className="py-1 text-right">Total do item</th>
                          </tr>
                        </thead>
                        <tbody>
                          {venda.itens.map((item) => (
                            <tr key={item.produtoId} className="text-slate-700">
                              <td className="py-1 pr-4">{item.produtoNome}</td>
                              <td className="py-1 pr-4 text-right">{item.quantidade}</td>
                              <td className="py-1 pr-4 text-right">{formatBRL(item.precoUnitario)}</td>
                              <td className="py-1 text-right font-medium">{formatBRL(item.totalItem)}</td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </td>
                  </tr>
                )}
              </Fragment>
            ))}
            {vendas.length === 0 && (
              <tr>
                <td colSpan={6} className="px-4 py-8 text-center text-slate-400">
                  Nenhuma venda registrada.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  )
}
