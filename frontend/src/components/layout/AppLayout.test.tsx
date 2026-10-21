import { render, screen } from '@testing-library/react'
import { createMemoryRouter, RouterProvider } from 'react-router'
import { describe, expect, it } from 'vitest'

import { routes } from '@/app/routes'

describe('layout', () => {
  it('mantém o cabeçalho e a barra na busca', () => {
    const memoryRouter = createMemoryRouter(routes, {
      initialEntries: ['/busca'],
    })

    render(<RouterProvider router={memoryRouter} />)

    expect(screen.getByText('YupAchei')).toBeInTheDocument()
    expect(screen.getByRole('link', { name: 'Início' })).toBeInTheDocument()
    expect(screen.getByRole('link', { name: 'Buscar' })).toBeInTheDocument()
  })
})
