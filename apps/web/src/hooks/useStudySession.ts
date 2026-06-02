import { useCallback, useEffect, useRef, useState } from 'react'
import { completeStudySession, getUserStats, reviewCard, startStudySession } from '../api/learning'
import { getApiErrorMessage } from '../auth/api'
import type {
  RatingBreakdown,
  ReviewRating,
  StudyCard,
  StudyProgress,
  StudySession,
  StudySessionSummary,
} from '../types/learning'

type StudyPhase = 'loading' | 'studying' | 'finishing' | 'summary' | 'empty' | 'error'

const emptyBreakdown: RatingBreakdown = { again: 0, hard: 0, good: 0, easy: 0 }

const breakdownKeyByRating: Record<ReviewRating, keyof RatingBreakdown> = {
  1: 'again',
  2: 'hard',
  3: 'good',
  4: 'easy',
}

export interface StudySessionController {
  phase: StudyPhase
  error: string | null
  currentCard: StudyCard | null
  isAnswerRevealed: boolean
  isSubmitting: boolean
  progress: StudyProgress
  breakdown: RatingBreakdown
  summary: StudySessionSummary | null
  streakDays: number | null
  reveal: () => void
  rate: (rating: ReviewRating) => void
  skip: () => void
  restart: () => void
}

export function useStudySession(deckId: string): StudySessionController {
  const [phase, setPhase] = useState<StudyPhase>('loading')
  const [error, setError] = useState<string | null>(null)
  const [session, setSession] = useState<StudySession | null>(null)
  const [currentIndex, setCurrentIndex] = useState(0)
  const [isAnswerRevealed, setIsAnswerRevealed] = useState(false)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [breakdown, setBreakdown] = useState<RatingBreakdown>(emptyBreakdown)
  const [skippedCount, setSkippedCount] = useState(0)
  const [summary, setSummary] = useState<StudySessionSummary | null>(null)
  const [streakDays, setStreakDays] = useState<number | null>(null)
  const [reloadToken, setReloadToken] = useState(0)
  const cardShownAt = useRef(0)

  const cards = session?.dueCards ?? []
  const currentCard = cards[currentIndex] ?? null
  const reviewedCount = breakdown.again + breakdown.hard + breakdown.good + breakdown.easy

  useEffect(() => {
    let active = true

    const load = async () => {
      try {
        const started = await startStudySession(deckId)
        if (!active) {
          return
        }
        setSession(started)
        setCurrentIndex(0)
        setIsAnswerRevealed(false)
        setBreakdown(emptyBreakdown)
        setSkippedCount(0)
        setSummary(null)
        setStreakDays(null)
        setError(null)
        cardShownAt.current = Date.now()
        setPhase(started.dueCards.length === 0 ? 'empty' : 'studying')
      } catch (cause) {
        if (active) {
          setError(getApiErrorMessage(cause))
          setPhase('error')
        }
      }
    }

    void load()

    return () => {
      active = false
    }
  }, [deckId, reloadToken])

  const restart = useCallback(() => {
    setPhase('loading')
    setReloadToken((token) => token + 1)
  }, [])

  const finish = useCallback(async (sessionId: string) => {
    setPhase('finishing')

    try {
      const completed = await completeStudySession(sessionId)
      setSummary(completed)
      setPhase('summary')
      getUserStats()
        .then((stats) => setStreakDays(stats.currentStreakDays))
        .catch(() => setStreakDays(null))
    } catch (cause) {
      setError(getApiErrorMessage(cause))
      setPhase('error')
    }
  }, [])

  const advance = useCallback(() => {
    const nextIndex = currentIndex + 1

    if (nextIndex >= cards.length) {
      if (session) {
        void finish(session.id)
      }
      return
    }

    setCurrentIndex(nextIndex)
    setIsAnswerRevealed(false)
    cardShownAt.current = Date.now()
  }, [currentIndex, cards.length, session, finish])

  const reveal = useCallback(() => setIsAnswerRevealed(true), [])

  const rate = useCallback(async (rating: ReviewRating) => {
    if (!session || !currentCard || isSubmitting) {
      return
    }

    setIsSubmitting(true)

    try {
      await reviewCard(session.id, {
        cardId: currentCard.id,
        rating,
        reviewDurationMs: Date.now() - cardShownAt.current,
      })
      setBreakdown((prev) => ({ ...prev, [breakdownKeyByRating[rating]]: prev[breakdownKeyByRating[rating]] + 1 }))
      advance()
    } catch (cause) {
      setError(getApiErrorMessage(cause))
      setPhase('error')
    } finally {
      setIsSubmitting(false)
    }
  }, [session, currentCard, isSubmitting, advance])

  const skip = useCallback(() => {
    if (isSubmitting) {
      return
    }

    setSkippedCount((count) => count + 1)
    advance()
  }, [isSubmitting, advance])

  const total = cards.length
  const progress: StudyProgress = {
    current: Math.min(currentIndex + 1, total),
    total,
    done: reviewedCount,
    remaining: total - currentIndex,
    skipped: skippedCount,
    percent: total === 0 ? 0 : Math.round((currentIndex / total) * 100),
  }

  return {
    phase,
    error,
    currentCard,
    isAnswerRevealed,
    isSubmitting,
    progress,
    breakdown,
    summary,
    streakDays,
    reveal,
    rate: (rating) => void rate(rating),
    skip,
    restart,
  }
}
