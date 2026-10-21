import { NavLink, Outlet } from 'react-router'

export function AppLayout() {
  return (
    <div className="flex min-h-dvh flex-col">
      <header className="flex items-center justify-between border-b border-gray-100 px-4 py-3">
      <div>
        <p className="text-sm font-semibold text-brand">YupAchei</p>
        <p className="text-xs text-gray-500">Porto Velho, RO</p>
      </div>
      <NavLink to="/busca" className="text-sm text-gray-700">
        Buscar
      </NavLink>
      </header>
      <main className="flex-1 px-4 py-4 pb-24">
        <Outlet />
      </main>
      <nav
        aria-label="Principal"
        className="fixed inset-x-0 bottom-0 border-t border-gray-100 bg-white px-4 py-3"
      >
        <NavLink
          to="/"
          end
          className={({ isActive }) =>
            isActive ? 'text-sm font-semibold text-brand' : 'text-sm text-gray-500'
          }
        >
          Início
        </NavLink>
        <span className="ml-4 text-sm text-gray-400">Eventos</span>
        <span className="ml-4 text-sm text-gray-400">Favoritos</span>
        <span className="ml-4 text-sm text-gray-400">Perfil</span>
      </nav>
    </div>
  )
}
