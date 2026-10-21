import locaisJson from '@/mocks/locais.json'

import { oneOf } from '@/lib/oneOf'
import { listCategories } from '@/features/categories/categories'
import { findTag } from '@/features/tags/tags'

const placeKinds = ['publico', 'comercial'] as const
const placeStatuses = ['aprovado', 'em_revisao'] as const
const weekdays = ['seg', 'ter', 'qua', 'qui', 'sex', 'sab', 'dom'] as const

export type PlaceKind = (typeof placeKinds)[number]
export type PlaceStatus = (typeof placeStatuses)[number]
export type Weekday = (typeof weekdays)[number]

export interface Address {
  logradouro: string | null
  numero: string
  bairro: string | null
  cidade: string
  uf: string
  cep: string | null
}

export interface Coordinates {
  lat: number
  lng: number
}

export interface OpeningHours {
  dia: Weekday
  abre: string
  fecha: string
}

export interface Contact {
  instagram: string | null
  whatsapp: string | null
  telefone: string | null
  email: string | null
  site: string | null
}

export interface PlaceImages {
  principal: string
  galeria: string[]
}

export interface Rating {
  media: number
  total: number
}

export interface Verification {
  endereco: boolean
  horario: boolean
  coordenadas: boolean
  fontes: string[]
  observacao?: string
}

export interface Place {
  id: string
  slug: string
  nome: string
  categoriaId: string
  tipo: PlaceKind
  descricao: string
  endereco: Address
  coordenadas: Coordinates
  faixaPreco: number
  aberto24h: boolean
  horarios: OpeningHours[]
  observacoes?: string
  tags: string[]
  contato: Contact
  imagens: PlaceImages
  avaliacao: Rating
  indicadoPor: string
  patrocinado: boolean
  status: PlaceStatus
  verificacao: Verification
  atualizadoEm: string
}

function toPlace(place: (typeof locaisJson)[number]): Place {
  const categoria = listCategories('local').find((item) => item.id === place.categoriaId)

  if (!categoria) {
    throw new Error(`Categoria de local desconhecida: ${place.categoriaId}`)
  }

  for (const tagId of place.tags) {
    if (!findTag(tagId)) {
      throw new Error(`Tag de local desconhecida: ${tagId}`)
    }
  }

  return {
    ...place,
    categoriaId: categoria.id,
    tipo: oneOf(place.tipo, placeKinds, 'Tipo de local'),
    status: oneOf(place.status, placeStatuses, 'Status de local'),
    horarios: place.horarios.map((horario) => ({
      ...horario,
      dia: oneOf(horario.dia, weekdays, 'Dia de funcionamento'),
    })),
  }
}

export const places: readonly Place[] = locaisJson.map(toPlace)

export function listPlaces(): readonly Place[] {
  return places.filter((place) => place.status === 'aprovado')
}

export function findPlace(id: string): Place | null {
  return listPlaces().find((place) => place.id === id) ?? null
}

export function listPlacesByCategory(categoriaId: string, bairro?: string): readonly Place[] {
  const inCategory = listPlaces().filter((place) => place.categoriaId === categoriaId)

  if (!bairro) {
    return inCategory
  }

  return inCategory.filter((place) => place.endereco.bairro === bairro)
}

export function listNeighborhoods(categoriaId: string): string[] {
  const names = listPlacesByCategory(categoriaId)
    .map((place) => place.endereco.bairro)
    .filter((bairro): bairro is string => bairro !== null)

  return [...new Set(names)].sort((a, b) => a.localeCompare(b, 'pt-BR'))
}
