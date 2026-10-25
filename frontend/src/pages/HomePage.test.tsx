import { render, screen } from '@testing-library/react'
import { MemoryRouter } from 'react-router'
import { describe, expect, it } from 'vitest'

import { HomePage } from '@/pages/HomePage'
import { listPlaces } from '@/features/places/places'

describe('HomePage', () => {
  it('lista as categorias de local', () => {
    render(
      <MemoryRouter>
        <HomePage />
      </MemoryRouter>,
    )

    const names = screen
      .getAllByRole('link')
      .filter((link) => link.getAttribute('href')?.startsWith('/categoria/'))
      .map((link) => link.textContent)

    expect(names).toEqual([
      'Cafeterias',
      'Restaurantes',
      'Bares',
      'Cultura',
      'Passeios',
      'Esportes',
    ])
    expect(screen.getByRole('link', { name: 'Restaurantes' })).toHaveAttribute(
      'href',
      '/categoria/restaurantes',
    )
  })

  it('lista os locais aprovados', () => {
    render(
      <MemoryRouter>
        <HomePage />
      </MemoryRouter>,
    )
    const placeLinks = screen
      .getAllByRole('link')
      .filter((link) => link.getAttribute('href')?.startsWith('/local/'))
    expect(placeLinks).toHaveLength(listPlaces().length)
    expect(screen.getByRole('link', { name: 'Recanto do Tambaqui' })).toHaveAttribute(
      'href',
      '/local/loc-016',
    )
    expect(
      screen.queryByRole('link', { name: 'Mercado Central de Porto Velho' }),
    ).not.toBeInTheDocument()
  })
})
