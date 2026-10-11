import { NavLink, Navigate, Route, Routes } from 'react-router-dom'
import CustomersPage from './pages/CustomersPage'
import NewSalePage from './pages/NewSalePage'
import ProductsPage from './pages/ProductsPage'
import SalesPage from './pages/SalesPage'

const links = [
  { to: '/sales/new', label: 'Nova Venda' },
  { to: '/sales', label: 'Vendas', end: true },
  { to: '/customers', label: 'Clientes' },
  { to: '/products', label: 'Produtos' },
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
          <Route path="/" element={<Navigate to="/sales/new" replace />} />
          <Route path="/sales/new" element={<NewSalePage />} />
          <Route path="/sales" element={<SalesPage />} />
          <Route path="/customers" element={<CustomersPage />} />
          <Route path="/products" element={<ProductsPage />} />
        </Routes>
      </main>
    </div>
  )
}
