import { render, screen } from '@testing-library/react'
import { createMemoryRouter, RouterProvider } from 'react-router'
import { describe, expect, it, vi } from 'vitest'

import { routes } from '@/app/routes'

describe('CategoryPage', () => {
  it('filtra os passeios do Centro', () => {
    const memoryRouter = createMemoryRouter(routes, {
      initialEntries: ['/categoria/passeios?bairro=Centro'],
    })

    render(<RouterProvider router={memoryRouter} />)

    expect(screen.getByRole('link', { name: "Praça das Três Caixas d'Água" })).toBeInTheDocument()
    expect(
      screen.queryByRole('link', {
        name: 'Parque Natural de Porto Velho (Parque Ecológico)',
      }),
    ).not.toBeInTheDocument()
  })

  it('filtra os eventos de cultura pela data', () => {
    vi.useFakeTimers()
    vi.setSystemTime(new Date('2026-10-21T15:00:00-04:00'))

    try {
      const memoryRouter = createMemoryRouter(routes, {
        initialEntries: ['/categoria/cultura?data=2026-10-24'],
      })

      render(<RouterProvider router={memoryRouter} />)

      expect(screen.getByRole('link', { name: 'Cinema ao Ar Livre' })).toBeInTheDocument()
      expect(
        screen.queryByRole('link', { name: 'Exposição Arte Amazônica' }),
      ).not.toBeInTheDocument()
    } finally {
      vi.useRealTimers()
    }
  })
})
