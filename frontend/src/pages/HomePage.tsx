import { Link } from 'react-router'

import { listCategories } from '@/features/categories/categories'
import { listPlaces } from '@/features/places/places'
import { PlaceCard } from '@/features/places/components/PlaceCard'

export function HomePage() {
  const categories = listCategories('local')
  const places = listPlaces()

  return (
    <div className="flex flex-col gap-8">
      <section>
        <h1 className="text-xl font-semibold">O que você procura hoje?</h1>
        <ul className="mt-4 flex flex-col gap-2">
          {categories.map((category) => (
            <li key={category.id}>
              <Link
                to={`/categoria/${category.id}`}
                className="flex items-center gap-3 rounded-xl border border-gray-100 px-3 py-2"
              >
                <span
                  aria-hidden="true"
                  className="size-3 rounded-full"
                  style={{ backgroundColor: category.cor }}
                />
                {category.nome}
              </Link>
            </li>
          ))}
        </ul>
      </section>

      <section>
        <h2 className="text-lg font-semibold">Descubra em Porto Velho</h2>
        <ul className="mt-4 flex flex-col gap-2">
          {places.map((place) => (
            <li key={place.id}>
              <PlaceCard place={place} />
            </li>
          ))}
        </ul>
      </section>
    </div>
  )
}
