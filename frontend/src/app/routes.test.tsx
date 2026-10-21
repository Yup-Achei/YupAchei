import { render, screen } from '@testing-library/react'
import { createMemoryRouter, RouterProvider } from 'react-router'
import { describe, it, expect } from 'vitest'


import { routes } from '@/app/routes';

function renderRoute(path: string) {
  const memoryRouter = createMemoryRouter(routes, {
    initialEntries: [path],
  })

  render(<RouterProvider router={memoryRouter} />)
}

describe('rotas', () => {
  it('deve abrir a página inicial', () => {
    renderRoute('/')

    expect(screen.getByRole('heading', { name: 'HomePage' })).toBeInTheDocument()
  })

  it('deve abrir a categoria pelo slug', () => {
    renderRoute('/categoria/bares')

    expect(screen.getByRole('heading', { name: 'Categoria bares' })).toBeInTheDocument()
  })

  it('deve abrir o local pelo id', () => {
    renderRoute('/local/1')

    expect(screen.getByRole('heading', { name: 'Lugar 1'})).toBeInTheDocument()
  })

  it('deve abrir o evento pelo id', () => {
    renderRoute('/evento/1')

    expect(screen.getByRole('heading', { name: 'Evento 1' })).toBeInTheDocument()
  })

  it('deve abrir a página de busca', () => {
    renderRoute('/busca')

    expect(screen.getByRole('heading', { name: 'Busca' })).toBeInTheDocument()
  })
})
