import { apiClient } from '../auth/api'
import type {
  CardReview,
  ReviewCardInput,
  StudySession,
  StudySessionSummary,
  UserStats,
} from '../types/learning'

export async function startStudySession(deckId: string) {
  const response = await apiClient.post<StudySession>(`/api/v1/decks/${deckId}/sessions/start`)
  return response.data
}

export async function reviewCard(sessionId: string, input: ReviewCardInput) {
  const response = await apiClient.post<CardReview>(`/api/v1/sessions/${sessionId}/review`, input)
  return response.data
}

export async function completeStudySession(sessionId: string) {
  const response = await apiClient.post<StudySessionSummary>(`/api/v1/sessions/${sessionId}/complete`)
  return response.data
}

export async function getUserStats() {
  const response = await apiClient.get<UserStats>('/api/v1/stats')
  return response.data
}
