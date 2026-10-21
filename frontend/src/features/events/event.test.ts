import { describe, expect, it } from 'vitest'

import { findEvent, listEvents } from '@/features/events/events'

const now = new Date('2026-10-21T15:00:00-04:00')

describe('eventos', () => {
  it('esconde o evento que já terminou', () => {
    expect(listEvents(now).map((event) => event.id)).toEqual([
      'evt-002',
      'evt-003',
      'evt-004',
      'evt-005',
      'evt-006',
      'evt-007',
      'evt-008',
    ])
    expect(findEvent('evt-001', now)).toBeNull()
  })

  it('encontra o evento sem local cadastrado', () => {
    expect(findEvent('evt-008', now)).toMatchObject({
      titulo: 'Forró nos Mirantes',
      localId: null,
      localLivre: { nome: 'Mirantes I, II e III' },
    })
  })
})
