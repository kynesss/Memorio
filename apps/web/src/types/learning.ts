import type { CardType } from './flashcards'

export const ReviewRating = {
  Again: 1,
  Hard: 2,
  Good: 3,
  Easy: 4,
} as const

export type ReviewRating = (typeof ReviewRating)[keyof typeof ReviewRating]

export type LearningState = 0 | 1 | 2 | 3

export interface StudyCard {
  id: string
  deckId: string
  front: string
  back: string
  tags: string | null
  type: CardType
  dueAt: string
  state: LearningState
  stability: number | null
  difficulty: number | null
}

export interface StudySession {
  id: string
  deckId: string
  startedAt: string
  dueCards: StudyCard[]
}

export interface ReviewCardInput {
  cardId: string
  rating: ReviewRating
  reviewDurationMs?: number
}

export interface CardReview {
  cardId: string
  rating: ReviewRating
  reviewedAt: string
  nextReviewAt: string
  state: LearningState
  stability: number | null
  difficulty: number | null
}

export interface StudySessionSummary {
  id: string
  deckId: string
  startedAt: string
  completedAt: string
  reviewedCards: number
  totalReviews: number
  studyTimeSeconds: number
}

export interface UserStats {
  currentStreakDays: number
  masteryPercentage: number
  totalStudyTimeSeconds: number
  completedSessions: number
  totalReviews: number
}

export type RatingKey = 'again' | 'hard' | 'good' | 'easy'

export type RatingBreakdown = Record<RatingKey, number>

export interface StudyProgress {
  current: number
  total: number
  done: number
  remaining: number
  skipped: number
  percent: number
}
