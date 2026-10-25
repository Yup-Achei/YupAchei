import { Link } from 'react-router'

import type { Place } from '@/features/places/places'

interface PlaceCardProps {
  place: Place
}

export function PlaceCard({ place }: PlaceCardProps) {
  const neighborhood = place.endereco.bairro ?? 'Bairro não informado'

  return (
    <Link
      to={`/local/${place.id}`}
      aria-label={place.nome}
      className="flex items-center gap-3 rounded-xl border border-gray-100 p-2"
    >
      <img
        src={place.imagens.principal}
        alt=""
        className="size-16 rounded-lg object-cover"
      />
      <span>
        <span className="block text-sm font-semibold">{place.nome}</span>
        <span className="block text-xs text-gray-500">{neighborhood}</span>
      </span>
    </Link>
  )
}
