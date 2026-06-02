import { useTranslation } from 'react-i18next'
import type { ReviewRating } from '../../types/learning'
import { ratingOrder } from './ratings'

interface RatingButtonsProps {
  isSubmitting: boolean
  onRate: (rating: ReviewRating) => void
}

export function RatingButtons({ isSubmitting, onRate }: RatingButtonsProps) {
  const { t } = useTranslation()

  return (
    <div className="space-y-4">
      <p className="text-center text-sm text-memorio-muted">{t('study.ratingQuestion')}</p>

      <div className="grid grid-cols-2 gap-3 sm:grid-cols-4">
        {ratingOrder.map((rating) => (
          <button
            key={rating.key}
            type="button"
            disabled={isSubmitting}
            onClick={() => onRate(rating.value)}
            className={`flex flex-col items-center justify-center gap-1 rounded-xl border bg-memorio-panel py-3.5 transition disabled:cursor-not-allowed disabled:opacity-50 ${rating.buttonClass}`}
          >
            <span className="text-sm font-bold">{t(`study.ratings.${rating.key}.label`)}</span>
            <span className="text-xs text-memorio-subtle">{t(`study.ratings.${rating.key}.hint`)}</span>
          </button>
        ))}
      </div>

      <p className="text-center text-xs text-memorio-subtle">
        {t('study.shortcuts')}{' '}
        {ratingOrder.map((rating, index) => `${index + 1}=${t(`study.ratings.${rating.key}.label`)}`).join('   ')}
      </p>
    </div>
  )
}
