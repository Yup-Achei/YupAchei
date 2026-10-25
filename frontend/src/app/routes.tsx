import type { RouteObject } from 'react-router'

import { AppLayout } from '@/components/layout/AppLayout'

import { HomePage } from '@/pages/HomePage'
import { CategoryPage } from '@/pages/CategoryPage'
import { EventDetailPage } from '@/pages/EventDetailPage'
import { PlaceDetailPage } from '@/pages/PlaceDetailPage'
import { SearchPage } from '@/pages/SearchPage'

export const routes: RouteObject[] = [
  {
    path: '/',
    Component: AppLayout,
    children: [
      { index: true, Component: HomePage },
      { path: '/categoria/:slug', Component: CategoryPage },
      { path: '/evento/:id', Component: EventDetailPage },
      { path: '/local/:id', Component: PlaceDetailPage },
      { path: '/busca', Component: SearchPage },
    ]
  }
]
