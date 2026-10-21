import { render, screen } from '@testing-library/react'
import { MemoryRouter } from 'react-router'
import { describe, expect, it } from 'vitest'

import { PlaceCard } from '@/features/places/components/PlaceCard'
import { findPlace } from '@/features/places/places'

describe('PlaceCard', () => {
  it('mostra o nome e o bairro do local', () => {
    const place = findPlace('loc-016')

    if (!place) {
      throw new Error('Local loc-016 não encontrado')
    }

    render(
      <MemoryRouter>
        <PlaceCard place={place} />
      </MemoryRouter>,
    )

    expect(screen.getByRole('link', { name: 'Recanto do Tambaqui' })).toHaveAttribute(
      'href',
      '/local/loc-016',
    )
    expect(screen.getByText('Vila Candelária')).toBeInTheDocument()
  })
})
