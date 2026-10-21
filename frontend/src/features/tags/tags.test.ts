import { describe, expect, it } from 'vitest'

import { findTag, listTags } from '@/features/tags/tags'

describe('tags', () => {
  it('lista as dezesseis tags na ordem do mock', () => {
    expect(listTags().map((tag) => tag.id)).toEqual([
      'gratuito',
      'familia',
      'pet-friendly',
      'sem-pets',
      'acessivel',
      'estacionamento',
      'wifi',
      'musica-ao-vivo',
      'ar-livre',
      'por-do-sol',
      'vista-rio',
      'comida-regional',
      'vegano',
      'trabalhar',
      'historico',
      'climatizado',
    ])
  })

  it('encontra a tag pelo id', () => {
    expect(findTag('wifi')).toMatchObject({ nome: 'Wi-Fi', icone: 'wifi' })
    expect(findTag('inexistente')).toBeNull()
  })
})
