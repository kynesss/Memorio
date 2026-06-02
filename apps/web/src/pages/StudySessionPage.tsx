import { useEffect } from 'react'
import { useTranslation } from 'react-i18next'
import { useNavigate, useParams } from 'react-router-dom'
import { Button } from '../components/common/Button'
import { EmptyState } from '../components/common/EmptyState'
import { PageLoader } from '../components/common/Spinner'
import { AppLayout } from '../components/layout/AppLayout'
import { RatingButtons } from '../components/study/RatingButtons'
import { StudyCard } from '../components/study/StudyCard'
import { StudyHeader } from '../components/study/StudyHeader'
import { StudySummary } from '../components/study/StudySummary'
import { useCurrentUser } from '../hooks/useCurrentUser'
import { useDeck } from '../hooks/useDeck'
import { useStudySession } from '../hooks/useStudySession'
import type { ReviewRating } from '../types/learning'

export function StudySessionPage() {
  const { t } = useTranslation()
  const { deckId = '' } = useParams()
  const navigate = useNavigate()

  const user = useCurrentUser()
  const deck = useDeck(deckId)
  const session = useStudySession(deckId)

  const email = user.data?.email ?? ''
  const deckName = deck.data?.name ?? ''

  const { phase, isAnswerRevealed, reveal, rate, skip } = session

  useEffect(() => {
    if (phase !== 'studying') {
      return
    }

    const handleKeyDown = (event: KeyboardEvent) => {
      if (!isAnswerRevealed) {
        if (event.code === 'Space') {
          event.preventDefault()
          reveal()
        }
        return
      }

      if (['1', '2', '3', '4'].includes(event.key)) {
        rate(Number(event.key) as ReviewRating)
      }
    }

    window.addEventListener('keydown', handleKeyDown)
    return () => window.removeEventListener('keydown', handleKeyDown)
  }, [phase, isAnswerRevealed, reveal, rate])

  const exitToDeck = () => navigate(`/decks/${deckId}`)
  const goToDashboard = () => navigate('/dashboard')

  return (
    <AppLayout email={email}>
      {phase === 'loading' || phase === 'finishing' ? (
        <PageLoader />
      ) : phase === 'error' ? (
        <EmptyState
          icon="⚠️"
          title={t('study.error.title')}
          description={session.error ?? t('errors.unexpected')}
          action={<Button onClick={session.restart}>{t('study.error.retry')}</Button>}
        />
      ) : phase === 'empty' ? (
        <EmptyState
          icon="✅"
          title={t('study.empty.title')}
          description={t('study.empty.description')}
          action={<Button onClick={exitToDeck}>{t('decks.backToDashboard')}</Button>}
        />
      ) : phase === 'summary' && session.summary ? (
        <StudySummary
          summary={session.summary}
          breakdown={session.breakdown}
          streakDays={session.streakDays}
          deckName={deckName}
          onRestart={session.restart}
          onBackToDashboard={goToDashboard}
        />
      ) : session.currentCard ? (
        <div className="mx-auto max-w-3xl space-y-6">
          <StudyHeader deckName={deckName} progress={session.progress} onExit={exitToDeck} />

          <StudyCard card={session.currentCard} isAnswerRevealed={isAnswerRevealed} />

          {isAnswerRevealed ? (
            <RatingButtons isSubmitting={session.isSubmitting} onRate={rate} />
          ) : (
            <div className="space-y-3">
              <Button className="w-full" onClick={reveal}>{t('study.showAnswer')}</Button>
              <button
                type="button"
                onClick={skip}
                className="block w-full text-center text-sm text-memorio-subtle transition hover:text-memorio-muted"
              >
                {t('study.skip')}
              </button>
            </div>
          )}
        </div>
      ) : (
        <PageLoader />
      )}
    </AppLayout>
  )
}
