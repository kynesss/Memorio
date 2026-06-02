import { Link } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import type { Deck } from '../../types/flashcards'
import { formatRelativeDate } from '../../utils/format'
import { Badge } from '../common/Badge'

interface FeaturedDeckProps {
  deck: Deck
}

export function FeaturedDeck({ deck }: FeaturedDeckProps) {
  const { i18n, t } = useTranslation()
  const language = i18n.resolvedLanguage ?? i18n.language

  return (
    <Link
      to={`/decks/${deck.id}`}
      className="group relative flex overflow-hidden rounded-2xl border border-memorio-border bg-[#17122b] p-7 transition hover:border-memorio-primary before:absolute before:inset-x-0 before:top-0 before:h-0.75 before:bg-memorio-primary before:content-['']"
    >
      <div className="pointer-events-none absolute -right-12 top-1/2 size-56 -translate-y-1/2 rounded-full bg-memorio-primary/15 blur-2xl" />
      <div className="relative flex flex-1 items-center justify-between gap-6">
        <div>
          <Badge className="bg-memorio-primary/20 text-memorio-primary-light">{t('decks.latest')}</Badge>
          <h3 className="mt-3 text-2xl font-bold text-memorio-text">{deck.name}</h3>
          <p className="mt-1 text-sm text-memorio-muted">
            {deck.description ?? t('decks.created', { date: formatRelativeDate(deck.createdAt, language) })}
          </p>
        </div>
        <span className="shrink-0 rounded-[10px] bg-memorio-primary px-5 py-2.5 text-sm font-semibold text-white transition group-hover:bg-memorio-primary-light">
          {t('decks.open')}
        </span>
      </div>
    </Link>
  )
}
