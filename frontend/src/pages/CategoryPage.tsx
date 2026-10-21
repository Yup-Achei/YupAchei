import { Link, useParams, useSearchParams } from 'react-router'

import { listCategories } from '@/features/categories/categories'
import { listEventsByCategory } from '@/features/events/events'
import { listNeighborhoods, listPlacesByCategory } from '@/features/places/places'
import { PlaceCard } from '@/features/places/components/PlaceCard'

export function CategoryPage() {
  const { slug = '' } = useParams()
  const [searchParams, setSearchParams] = useSearchParams()
  const bairro = searchParams.get('bairro') ?? ''
  const data = searchParams.get('data') ?? ''
  const category = listCategories('local').find((item) => item.id === slug)

  if (!category) {
    return <h1>Categoria não encontrada</h1>
  }

  function replaceParams(next: { bairro?: string; data?: string }) {
    const params: Record<string, string> = {}
    const nextBairro = next.bairro ?? bairro
    const nextData = next.data ?? data

    if (nextBairro) {
      params.bairro = nextBairro
    }

    if (nextData) {
      params.data = nextData
    }

    setSearchParams(params)
  }

  const places = listPlacesByCategory(category.id, bairro || undefined)
  const neighborhoods = listNeighborhoods(category.id)
  const categoryEvents = listEventsByCategory(category.id, data || undefined)
  const hasEvents = listEventsByCategory(category.id).length > 0

  return (
    <section>
      <h1 className="text-xl font-semibold">{category.nome}</h1>
      <div className="mt-4 flex flex-wrap gap-2">
        <button
          type="button"
          onClick={() => replaceParams({ bairro: '' })}
          className={bairro === '' ? 'text-sm font-semibold text-brand' : 'text-sm text-gray-500'}
        >
          Todos
        </button>
        {neighborhoods.map((name) => (
          <button
            key={name}
            type="button"
            onClick={() => replaceParams({ bairro: name })}
            className={
              bairro === name ? 'text-sm font-semibold text-brand' : 'text-sm text-gray-500'
            }
          >
            {name}
          </button>
        ))}
      </div>
      {places.length === 0 ? (
        <p className="mt-4 text-sm text-gray-500">Nenhum local nesta categoria.</p>
      ) : (
        <ul className="mt-4 flex flex-col gap-2">
          {places.map((place) => (
            <li key={place.id}>
              <PlaceCard place={place} />
            </li>
          ))}
        </ul>
      )}
      {hasEvents && (
        <div className="mt-8">
          <h2 className="text-lg font-semibold">Eventos</h2>
          <label className="mt-4 block text-sm text-gray-500">
            Data
            <input
              type="date"
              value={data}
              onChange={(event) => replaceParams({ data: event.target.value })}
              className="mt-1 block rounded-xl border border-gray-100 px-3 py-2"
            />
          </label>
          {categoryEvents.length === 0 ? (
            <p className="mt-4 text-sm text-gray-500">Nenhum evento nesta data.</p>
          ) : (
            <ul className="mt-4 flex flex-col gap-2">
              {categoryEvents.map((event) => (
                <li key={event.id}>
                  <Link
                    to={`/evento/${event.id}`}
                    className="block rounded-xl border border-gray-100 px-3 py-2"
                  >
                    {event.titulo}
                  </Link>
                </li>
              ))}
            </ul>
          )}
        </div>
      )}
    </section>
  )
}
