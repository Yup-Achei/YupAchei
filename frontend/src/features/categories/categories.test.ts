import { describe, expect, it } from 'vitest'

import { listCategories } from '@/features/categories/categories'

describe('categorias', () => {
  it('lista as categorias de local na ordem do mock', () => {
    expect(listCategories('local').map((category) => category.id)).toEqual([
      'cafeterias',
      'restaurantes',
      'bares',
      'cultura',
      'passeios',
      'esportes',
    ])
  })

  it('lista as categorias de evento na ordem do mock', () => {
    expect(listCategories('evento').map((category) => category.id)).toEqual([
      'cultura',
      'esportes',
      'musica',
      'festas',
      'gastronomia',
      'teatro',
    ])
  })
})
