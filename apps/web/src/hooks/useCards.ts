import { useCallback } from 'react'
import { getCards } from '../api/cards'
import { buildCardSearchFilter } from '../utils/format'
import { useQuery } from './useQuery'

export const CARDS_PAGE_SIZE = 10

export function useCards(deckId: string, page: number, search: string) {
  const fetcher = useCallback(
    () => getCards(deckId, {
      page,
      pageSize: CARDS_PAGE_SIZE,
      filters: buildCardSearchFilter(search),
      sorts: '-CreatedAt',
    }),
    [deckId, page, search],
  )

  return useQuery(fetcher)
}

export function useCardCount(deckId: string) {
  const fetcher = useCallback(async () => {
    const result = await getCards(deckId, { page: 1, pageSize: 1 })
    return result.totalCount
  }, [deckId])

  return useQuery(fetcher)
}
