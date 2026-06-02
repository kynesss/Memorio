import { useTranslation } from 'react-i18next'
import type { Deck } from '../../types/flashcards'
import { Button } from '../common/Button'

interface DeckDetailHeaderProps {
  deck: Deck
  cardCount: number
  onEdit: () => void
  onDelete: () => void
  onStudy: () => void
}

export function DeckDetailHeader({ deck, cardCount, onEdit, onDelete, onStudy }: DeckDetailHeaderProps) {
  const { t } = useTranslation()

  return (
    <section className="relative overflow-hidden rounded-2xl border border-memorio-border bg-[#17122b] px-8 py-7 before:absolute before:inset-x-0 before:top-0 before:h-0.75 before:bg-memorio-primary before:content-['']">
      <div className="pointer-events-none absolute -right-12 top-1/2 size-56 -translate-y-1/2 rounded-full bg-memorio-primary/15 blur-2xl" />
      <div className="relative flex flex-wrap items-start justify-between gap-6">
        <div>
          <h1 className="text-3xl font-bold text-memorio-text">{deck.name}</h1>
          <p className="mt-2 text-sm text-memorio-muted">
            {t('decks.cardTotal', { count: cardCount })}
            {deck.description ? ` · ${deck.description}` : ''}
          </p>
        </div>
        <div className="flex flex-wrap items-center gap-3">
          <Button variant="danger" onClick={onDelete}>{t('common.delete')}</Button>
          <Button variant="secondary" onClick={onEdit}>{t('decks.edit')}</Button>
          <Button onClick={onStudy}>{t('decks.studyNow')}</Button>
        </div>
      </div>
    </section>
  )
}
