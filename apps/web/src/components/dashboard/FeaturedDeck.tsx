import { Link } from 'react-router-dom'
import type { Deck } from '../../types/flashcards'
import { formatRelativeDate } from '../../utils/format'
import { Badge } from '../common/Badge'

interface FeaturedDeckProps {
  deck: Deck
}

export function FeaturedDeck({ deck }: FeaturedDeckProps) {
  return (
    <Link
      to={`/decks/${deck.id}`}
      className="group relative flex overflow-hidden rounded-2xl border border-memorio-border bg-[#17122b] p-7 transition hover:border-memorio-primary before:absolute before:inset-x-0 before:top-0 before:h-0.75 before:bg-memorio-primary before:content-['']"
    >
      <div className="pointer-events-none absolute -right-12 top-1/2 size-56 -translate-y-1/2 rounded-full bg-memorio-primary/15 blur-2xl" />
      <div className="relative flex flex-1 items-center justify-between gap-6">
        <div>
          <Badge className="bg-memorio-primary/20 text-memorio-primary-light">Latest</Badge>
          <h3 className="mt-3 text-2xl font-bold text-memorio-text">{deck.name}</h3>
          <p className="mt-1 text-sm text-memorio-muted">
            {deck.description ?? `Created ${formatRelativeDate(deck.createdAt)}`}
          </p>
        </div>
        <span className="shrink-0 rounded-[10px] bg-memorio-primary px-5 py-2.5 text-sm font-semibold text-white transition group-hover:bg-memorio-primary-light">
          Open →
        </span>
      </div>
    </Link>
  )
}
