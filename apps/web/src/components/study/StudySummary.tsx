import { useTranslation } from 'react-i18next'
import type { RatingBreakdown, StudySessionSummary } from '../../types/learning'
import { formatStudyDuration } from '../../utils/format'
import { Button } from '../common/Button'
import { ratingOrder } from './ratings'

interface StudySummaryProps {
  summary: StudySessionSummary
  breakdown: RatingBreakdown
  streakDays: number | null
  deckName: string
  onRestart: () => void
  onBackToDashboard: () => void
}

export function StudySummary({
  summary,
  breakdown,
  streakDays,
  deckName,
  onRestart,
  onBackToDashboard,
}: StudySummaryProps) {
  const { t } = useTranslation()

  const stats = [
    { value: String(summary.reviewedCards), label: t('study.summary.cardsReviewed'), color: '#8863f9' },
    { value: String(summary.totalReviews), label: t('study.summary.totalReviews'), color: '#3ecf8e' },
    { value: formatStudyDuration(summary.studyTimeSeconds), label: t('study.summary.timeSpent'), color: '#29c7e0' },
    { value: streakDays === null ? '—' : String(streakDays), label: t('study.summary.dayStreak'), color: '#f5b60c' },
  ]

  return (
    <div className="mx-auto max-w-3xl text-center">
      <div className="mx-auto flex size-20 items-center justify-center rounded-full border-2 border-memorio-success/40 bg-memorio-success/10 text-3xl text-memorio-success">
        ✓
      </div>

      <h1 className="mt-6 text-3xl font-bold text-memorio-text">{t('study.summary.title')}</h1>
      <p className="mt-2 text-sm text-memorio-muted">
        {t('study.summary.subtitle', { count: summary.reviewedCards, deck: deckName })}
      </p>

      <div className="mt-8 grid grid-cols-2 gap-4 sm:grid-cols-4">
        {stats.map((stat) => (
          <div
            key={stat.label}
            className="rounded-xl border border-memorio-border bg-memorio-panel px-5 py-4 text-left"
            style={{ borderTopColor: stat.color, borderTopWidth: 2 }}
          >
            <p className="text-2xl font-bold" style={{ color: stat.color }}>{stat.value}</p>
            <p className="mt-1 text-xs text-memorio-muted">{stat.label}</p>
          </div>
        ))}
      </div>

      <div className="mt-4 flex flex-wrap items-center justify-center gap-x-8 gap-y-2 rounded-xl border border-memorio-border bg-memorio-panel px-6 py-4 text-sm text-memorio-muted">
        {ratingOrder.map((rating) => (
          <span key={rating.key} className="inline-flex items-center gap-2">
            <span className="size-2 rounded-full" style={{ background: rating.color }} />
            {t(`study.ratings.${rating.key}.label`)}: {breakdown[rating.key]}
          </span>
        ))}
      </div>

      <div className="mt-8 flex flex-wrap justify-center gap-4">
        <Button onClick={onRestart}>{t('study.summary.studyAgain')}</Button>
        <Button variant="secondary" onClick={onBackToDashboard}>{t('study.summary.backToDashboard')}</Button>
      </div>

      {streakDays !== null && streakDays > 0 && (
        <div className="mt-6 inline-flex items-center gap-2 rounded-xl border border-memorio-warning/40 bg-memorio-warning/10 px-5 py-3 text-sm font-semibold text-memorio-warning">
          🔥 {t('study.summary.streakBanner', { count: streakDays })}
        </div>
      )}
    </div>
  )
}
