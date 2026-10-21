import tagsJson from '@/mocks/tags.json'

export interface Tag {
  id: string
  nome: string
  icone: string
}

export const tags = tagsJson satisfies readonly Tag[]

export function listTags(): readonly Tag[] {
  return tags
}

export function findTag(id: string): Tag | null {
  return tags.find((tag) => tag.id === id) ?? null
}
