import { Fragment, useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../services/api'
import type { Sale } from '../types'
import { formatBRL, formatCpf, formatDate } from '../utils/format'
import { Feedback, btnLink } from '../components/ui'

export default function SalesPage() {
  const [sales, setSales] = useState<Sale[]>([])
  const [expandedId, setExpandedId] = useState<number | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    api
      .get<Sale[]>('/sales')
      .then(setSales)
      .catch((e) => setError(e instanceof Error ? e.message : 'Erro ao carregar vendas.'))
  }, [])

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold text-slate-900">Vendas</h1>
        <Link to="/sales/new" className={btnLink}>
          + Nova venda
        </Link>
      </div>

      <Feedback error={error} />

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
            {sales.map((sale) => (
              <Fragment key={sale.id}>
                <tr className="hover:bg-slate-50">
                  <td className="px-4 py-3 font-medium text-slate-900">#{sale.id}</td>
                  <td className="px-4 py-3">{formatDate(sale.saleDate)}</td>
                  <td className="px-4 py-3">
                    <div className="font-medium text-slate-900">{sale.customerName}</div>
                    <div className="text-xs text-slate-500">{formatCpf(sale.customerCpf)}</div>
                  </td>
                  <td className="px-4 py-3 text-center">{sale.items.length}</td>
                  <td className="px-4 py-3 text-right font-semibold text-slate-900">
                    {formatBRL(sale.totalAmount)}
                  </td>
                  <td className="px-4 py-3 text-right">
                    <button
                      type="button"
                      className={btnLink}
                      onClick={() =>
                        setExpandedId(expandedId === sale.id ? null : sale.id)
                      }
                    >
                      {expandedId === sale.id ? 'Ocultar' : 'Detalhes'}
                    </button>
                  </td>
                </tr>
                {expandedId === sale.id && (
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
                          {sale.items.map((item) => (
                            <tr key={item.productId} className="text-slate-700">
                              <td className="py-1 pr-4">{item.productName}</td>
                              <td className="py-1 pr-4 text-right">{item.quantity}</td>
                              <td className="py-1 pr-4 text-right">{formatBRL(item.unitPrice)}</td>
                              <td className="py-1 text-right font-medium">{formatBRL(item.itemTotal)}</td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </td>
                  </tr>
                )}
              </Fragment>
            ))}
            {sales.length === 0 && (
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
