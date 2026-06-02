import { Link } from 'react-router-dom'
import type { Deck } from '../../types/flashcards'
import { formatRelativeDate } from '../../utils/format'

interface DeckCardProps {
  deck: Deck
  accent: string
}

export function DeckCard({ deck, accent }: DeckCardProps) {
  return (
    <Link
      to={`/decks/${deck.id}`}
      className="group flex flex-col rounded-xl border border-memorio-border bg-memorio-panel p-5 transition hover:border-memorio-primary"
      style={{ borderTopColor: accent, borderTopWidth: 2 }}
    >
      <h3 className="text-base font-bold text-memorio-text">{deck.name}</h3>
      <p className="mt-1 line-clamp-2 min-h-8 text-xs text-memorio-muted">
        {deck.description ?? 'No description'}
      </p>
      <div className="mt-5 flex items-center justify-between">
        <span className="text-xs text-memorio-subtle">Created {formatRelativeDate(deck.createdAt)}</span>
        <span className="rounded-lg border border-memorio-border px-3 py-1.5 text-xs font-semibold text-memorio-text transition group-hover:border-memorio-primary group-hover:text-memorio-primary-light">
          Open →
        </span>
      </div>
    </Link>
  )
}
