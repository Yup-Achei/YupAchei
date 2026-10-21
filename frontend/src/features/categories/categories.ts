import categoriasJson from '@/mocks/categorias.json'

import { oneOf } from '@/lib/oneOf'

const contentTypes = ['local', 'evento'] as const

export type ContentType = (typeof contentTypes)[number]

export interface Category {
  id: string
  nome: string
  tipos: ContentType[]
  icone: string
  cor: string
  ordem: number
}

export const categories: readonly Category[] = categoriasJson.map((category) => ({
  ...category,
  tipos: category.tipos.map((tipo) => oneOf(tipo, contentTypes, 'Tipo de categoria')),
}))

export function listCategories(tipo: ContentType): readonly Category[] {
  return categories
    .filter((category) => category.tipos.includes(tipo))
    .sort((a, b) => a.ordem - b.ordem)
}
