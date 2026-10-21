import { useParams } from 'react-router'

export function PlaceDetailPage() {
  const { id } = useParams()

  return <h1>Lugar { id }</h1>
}
