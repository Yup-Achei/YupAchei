import { useParams } from 'react-router'

export function CategoryPage() {
  const { slug } = useParams()

  return <h1>Categoria { slug }</h1>
}
