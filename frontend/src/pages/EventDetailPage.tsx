import { useParams } from 'react-router'

export function EventDetailPage() {
  const { id } = useParams()

  return <h1>Evento { id }</h1>
}
