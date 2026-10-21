import { describe, expect, it } from 'vitest'

import { findPlace, listPlaces, listPlacesByCategory, listNeighborhoods } from '@/features/places/places'

describe('locais', () => {
  it('lista só os locais aprovados', () => {
    const ids = listPlaces().map((place) => place.id)

    expect(ids).toHaveLength(17)
    expect(ids).not.toContain('loc-009')
    expect(ids).not.toContain('loc-019')
  })

  it('encontra um local aprovado pelo id', () => {
    expect(findPlace('loc-013')).toMatchObject({
      nome: 'Cinema do Porto Velho Shopping',
      tipo: 'comercial',
      categoriaId: 'cultura',
    })
    expect(findPlace('loc-009')).toBeNull()
  })

  it('lista os locais aprovados de uma categoria', () => {
    expect(listPlacesByCategory('restaurantes').map((place) => place.id)).toEqual([
      'loc-016',
      'loc-017',
    ])
  })

  it('filtra os locais da categoria por bairro', () => {
    expect(listPlacesByCategory('passeios', 'Centro').map((place) => place.id)).toEqual([
      'loc-002',
      'loc-003',
      'loc-014',
    ])
    expect(listNeighborhoods('passeios')).toEqual(['Centro', 'Panair'])
  })
})
