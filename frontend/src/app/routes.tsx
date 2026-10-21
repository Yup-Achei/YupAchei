import type { RouteObject } from 'react-router'

import { HomePage } from '@/pages/HomePage'
import { CategoryPage } from '@/pages/CategoryPage'
import { EventDetailPage } from '@/pages/EventDetailPage'
import { PlaceDetailPage } from '@/pages/PlaceDetailPage'
import { SearchPage } from '@/pages/SearchPage'

export const routes: RouteObject[] = [
  { path: '/', Component: HomePage },
  { path: '/categoria/:slug', Component: CategoryPage },
  { path: '/evento/:id', Component: EventDetailPage },
  { path: '/local/:id', Component: PlaceDetailPage },
  { path: '/busca', Component: SearchPage },
]
