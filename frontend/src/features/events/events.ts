import eventosJson from '@/mocks/eventos.json'

import { listCategories } from '@/features/categories/categories'
import { places, type Coordinates } from '@/features/places/places'
import { findTag } from '@/features/tags/tags'
import { oneOf } from '@/lib/oneOf'

const eventStatuses = ['aprovado', 'em_revisao'] as const
const documentTypes = ['MEI', 'CNPJ'] as const
const currencies = ['BRL'] as const

export type EventStatus = (typeof eventStatuses)[number]
export type DocumentType = (typeof documentTypes)[number]
export type Currency = (typeof currencies)[number]

export interface FreePlace {
  nome: string
  endereco: string | null
  bairro: string | null
  coordenadas: Coordinates
}

export interface EventPrice {
  gratuito: boolean
  valorMinimo: number | null
  moeda: Currency
}

export interface Organizer {
  nome: string
  tipoDocumento: DocumentType
  documento: string
}

export interface Event {
  id: string
  slug: string
  simulado: boolean
  titulo: string
  categoriaId: string
  descricao: string
  inicio: string
  fim: string
  localId: string | null
  localLivre: FreePlace | null
  preco: EventPrice
  linkIngresso: string | null
  imagem: string
  organizador: Organizer
  tags: string[]
  destaque: boolean
  patrocinado: boolean
  status: EventStatus
}

function toEvent(event: (typeof eventosJson)[number]): Event {
  const categoria = listCategories('evento').find((item) => item.id === event.categoriaId)

  if (!categoria) {
    throw new Error(`Categoria de evento desconhecida: ${event.categoriaId}`)
  }

  for (const tagId of event.tags) {
    if (!findTag(tagId)) {
      throw new Error(`Tag de evento desconhecida: ${tagId}`)
    }
  }

  const hasPlace = event.localId !== null
  const hasFreePlace = event.localLivre !== null

  if (hasPlace === hasFreePlace) {
    throw new Error(`Evento ${event.id} precisa de localId ou localLivre`)
  }

  if (event.localId !== null && !places.some((place) => place.id === event.localId)) {
    throw new Error(`Local do evento desconhecido: ${event.localId}`)
  }

  return {
    ...event,
    categoriaId: categoria.id,
    status: oneOf(event.status, eventStatuses, 'Status de evento'),
    localLivre: event.localLivre,
    preco: {
      ...event.preco,
      moeda: oneOf(event.preco.moeda, currencies, 'Moeda'),
    },
    organizador: {
      ...event.organizador,
      tipoDocumento: oneOf(
        event.organizador.tipoDocumento,
        documentTypes,
        'Tipo de documento',
      ),
    },
  }
}

export const events: readonly Event[] = eventosJson.map(toEvent)

export function listEvents(now = new Date()): readonly Event[] {
  return events
    .filter((event) => event.status === 'aprovado' && new Date(event.fim) >= now)
    .sort((a, b) => (a.inicio < b.inicio ? -1 : a.inicio > b.inicio ? 1 : 0))
}

export function findEvent(id: string, now = new Date()): Event | null {
  return listEvents(now).find((event) => event.id === id) ?? null
}

function dayKey(iso: string): string {
  return iso.slice(0, 10)
}


export function listEventsByCategory(
  categoriaId: string,
  date?: string,
  now = new Date(),
): readonly Event[] {
  const inCategory = listEvents(now).filter((event) => event.categoriaId === categoriaId)
  if (!date) {
    return inCategory
  }
  return inCategory.filter(
    (event) => dayKey(event.inicio) <= date && date <= dayKey(event.fim),
  )
}
