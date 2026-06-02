import { useCallback } from 'react'
import { getDeck } from '../api/decks'
import { useQuery } from './useQuery'

export function useDeck(deckId: string) {
  const fetcher = useCallback(() => getDeck(deckId), [deckId])
  return useQuery(fetcher)
}
