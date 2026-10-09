import { NavLink, Navigate, Route, Routes } from 'react-router-dom'
import ClientesPage from './pages/ClientesPage'
import NovaVendaPage from './pages/NovaVendaPage'
import ProdutosPage from './pages/ProdutosPage'
import VendasPage from './pages/VendasPage'

const links = [
  { to: '/vendas/nova', label: 'Nova Venda' },
  { to: '/vendas', label: 'Vendas', end: true },
  { to: '/clientes', label: 'Clientes' },
  { to: '/produtos', label: 'Produtos' },
]

export default function App() {
  return (
    <div className="min-h-screen">
      <header className="border-b border-slate-200 bg-white">
        <div className="mx-auto flex max-w-6xl items-center justify-between px-4 py-3">
          <span className="text-lg font-semibold text-slate-900">
            Controle de Vendas
          </span>
          <nav className="flex gap-1">
            {links.map((link) => (
              <NavLink
                key={link.to}
                to={link.to}
                end={link.end}
                className={({ isActive }) =>
                  `rounded-lg px-3 py-2 text-sm font-medium ${
                    isActive
                      ? 'bg-indigo-50 text-indigo-700'
                      : 'text-slate-600 hover:bg-slate-100'
                  }`
                }
              >
                {link.label}
              </NavLink>
            ))}
          </nav>
        </div>
      </header>

      <main className="mx-auto max-w-6xl px-4 py-6">
        <Routes>
          <Route path="/" element={<Navigate to="/vendas/nova" replace />} />
          <Route path="/vendas/nova" element={<NovaVendaPage />} />
          <Route path="/vendas" element={<VendasPage />} />
          <Route path="/clientes" element={<ClientesPage />} />
          <Route path="/produtos" element={<ProdutosPage />} />
        </Routes>
      </main>
    </div>
  )
}
