import { describe, expect, it } from 'vitest'

import { oneOf } from '@/lib/oneOf'

const contentTypes = ['local', 'evento'] as const

describe('oneOf', () => {
  it('aceita um valor da lista', () => {
    expect(oneOf('local', contentTypes, 'Tipo')).toBe('local')
  })

  it('recusa um valor fora da lista', () => {
    expect(() => oneOf('outro', contentTypes, 'Tipo')).toThrow(
      'Tipo desconhecido: outro',
    )
  })
})
