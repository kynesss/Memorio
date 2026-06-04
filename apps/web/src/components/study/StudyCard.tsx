import { useTranslation } from 'react-i18next'
import type { StudyCard as StudyCardModel } from '../../types/learning'
import { parseCardContent, type ParsedCardContent } from '../../utils/cardContent'
import { parseTags } from '../../utils/format'
import { Badge } from '../common/Badge'

interface StudyCardProps {
  card: StudyCardModel
  isAnswerRevealed: boolean
}

export function StudyCard({ card, isAnswerRevealed }: StudyCardProps) {
  const { t } = useTranslation()
  const tags = parseTags(card.tags)
  const front = parseCardContent(card.front)
  const back = parseCardContent(card.back)

  return (
    <div className="relative flex min-h-[340px] flex-col overflow-hidden rounded-2xl border border-memorio-border bg-memorio-panel px-8 py-7">
      <div className="pointer-events-none absolute -right-12 -top-12 size-72 rounded-full bg-memorio-primary/10 blur-3xl" />

      <div className="relative flex flex-1 flex-col">
        <span className="text-xs font-semibold uppercase tracking-wider text-memorio-subtle">{t('study.front')}</span>

        {isAnswerRevealed ? (
          <>
            <CardContent content={front} textClassName="text-2xl text-memorio-muted" />

            <hr className="my-6 border-memorio-border" />

            <span className="text-xs font-semibold uppercase tracking-wider text-memorio-primary-light">{t('study.back')}</span>
            <CardContent content={back} textClassName="text-2xl text-memorio-text" />

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
              <CardContent content={front} textClassName="text-5xl text-memorio-text" imageClassName="max-h-72" />
            </div>
            <p className="text-center text-sm text-memorio-subtle">{t('study.tapToReveal')}</p>
          </>
        )}
      </div>
    </div>
  )
}

function CardContent({
  content,
  textClassName,
  imageClassName = 'max-h-64',
}: {
  content: ParsedCardContent
  textClassName: string
  imageClassName?: string
}) {
  return (
    <div className="mt-4 flex w-full flex-col items-center gap-4">
      <p className={`whitespace-pre-line text-center font-bold ${textClassName}`}>{content.text}</p>
      {content.images.map((image) => (
        <img
          key={image.id}
          src={image.src}
          alt={image.fileName}
          className={`w-full max-w-xl rounded-xl border border-memorio-border bg-memorio-input object-contain ${imageClassName}`}
        />
      ))}
    </div>
  )
}
