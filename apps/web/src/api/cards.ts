import { apiClient } from '../auth/api'
import type { Card, CardInput, CardMedia, PagedResult } from '../types/flashcards'

const cardsPath = (deckId: string) => `/api/v1/decks/${deckId}/cards`

export interface CardQuery {
  page?: number
  pageSize?: number
  filters?: string
  sorts?: string
}

export async function getCards(deckId: string, query?: CardQuery) {
  const response = await apiClient.get<PagedResult<Card>>(cardsPath(deckId), { params: query })
  return response.data
}

export async function createCard(deckId: string, input: CardInput) {
  const response = await apiClient.post<Card>(cardsPath(deckId), input)
  return response.data
}

export async function updateCard(deckId: string, cardId: string, input: CardInput) {
  const response = await apiClient.put<Card>(`${cardsPath(deckId)}/${cardId}`, input)
  return response.data
}

export async function deleteCard(deckId: string, cardId: string) {
  await apiClient.delete(`${cardsPath(deckId)}/${cardId}`)
}

export async function uploadCardMedia(cardId: string, file: File) {
  const formData = new FormData()
  formData.append('file', file)

  const response = await apiClient.post<CardMedia>(`/api/v1/cards/${cardId}/media`, formData)
  return response.data
}

export async function deleteCardMedia(cardId: string, mediaId: string) {
  await apiClient.delete(`/api/v1/cards/${cardId}/media/${mediaId}`)
}
