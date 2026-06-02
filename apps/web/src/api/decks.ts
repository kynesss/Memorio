import { apiClient } from '../auth/api'
import type { Deck, DeckInput, PagedResult } from '../types/flashcards'

const basePath = '/api/v1/decks'

export interface DeckQuery {
  page?: number
  pageSize?: number
  sorts?: string
}

export async function getDecks(query?: DeckQuery) {
  const response = await apiClient.get<PagedResult<Deck>>(basePath, { params: query })
  return response.data
}

export async function getDeck(deckId: string) {
  const response = await apiClient.get<Deck>(`${basePath}/${deckId}`)
  return response.data
}

export async function createDeck(input: DeckInput) {
  const response = await apiClient.post<Deck>(basePath, input)
  return response.data
}

export async function updateDeck(deckId: string, input: DeckInput) {
  const response = await apiClient.put<Deck>(`${basePath}/${deckId}`, input)
  return response.data
}

export async function deleteDeck(deckId: string) {
  await apiClient.delete(`${basePath}/${deckId}`)
}
