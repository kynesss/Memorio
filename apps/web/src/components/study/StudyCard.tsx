import { useTranslation } from 'react-i18next'
import type { StudyCard as StudyCardModel } from '../../types/learning'
import { parseTags } from '../../utils/format'
import { Badge } from '../common/Badge'

interface StudyCardProps {
  card: StudyCardModel
  isAnswerRevealed: boolean
}

export function StudyCard({ card, isAnswerRevealed }: StudyCardProps) {
  const { t } = useTranslation()
  const tags = parseTags(card.tags)

  return (
    <div className="relative flex min-h-[340px] flex-col overflow-hidden rounded-2xl border border-memorio-border bg-memorio-panel px-8 py-7">
      <div className="pointer-events-none absolute -right-12 -top-12 size-72 rounded-full bg-memorio-primary/10 blur-3xl" />

      <div className="relative flex flex-1 flex-col">
        <span className="text-xs font-semibold uppercase tracking-wider text-memorio-subtle">{t('study.front')}</span>

        {isAnswerRevealed ? (
          <>
            <h2 className="mt-4 text-center text-2xl font-bold text-memorio-muted">{card.front}</h2>

            <hr className="my-6 border-memorio-border" />

            <span className="text-xs font-semibold uppercase tracking-wider text-memorio-primary-light">{t('study.back')}</span>
            <p className="mt-4 whitespace-pre-line text-center text-2xl font-bold text-memorio-text">{card.back}</p>

            {tags.length > 0 && (
              <div className="mt-6 flex flex-wrap gap-2">
                {tags.map((tag) => (
                  <Badge key={tag}>{tag}</Badge>
                ))}
              </div>
            )}
          </>
        ) : (
          <>
            <div className="flex flex-1 items-center justify-center">
              <h2 className="text-center text-5xl font-bold text-memorio-text">{card.front}</h2>
            </div>
            <p className="text-center text-sm text-memorio-subtle">{t('study.tapToReveal')}</p>
          </>
        )}
      </div>
    </div>
  )
}
