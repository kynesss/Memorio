import { useCallback } from 'react'
import { getDecks } from '../api/decks'
import { useQuery } from './useQuery'

export function useDecks() {
  const fetcher = useCallback(() => getDecks({ pageSize: 100, sorts: '-CreatedAt' }), [])
  return useQuery(fetcher)
}
