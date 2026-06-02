import type { Card } from '../../types/flashcards'
import { accentForIndex, parseTags } from '../../utils/format'
import { Badge } from '../common/Badge'

interface CardsTableProps {
  cards: Card[]
  onEdit: (card: Card) => void
  onDelete: (card: Card) => void
}

export function CardsTable({ cards, onEdit, onDelete }: CardsTableProps) {
  const { t } = useTranslation()

  return (
    <div className="overflow-x-auto rounded-xl border border-memorio-border">
      <table className="w-full border-collapse text-left">
        <thead>
          <tr className="bg-memorio-panel text-xs uppercase tracking-wide text-memorio-subtle">
            <th className="px-5 py-3 font-medium">{t('cards.table.front')}</th>
            <th className="px-5 py-3 font-medium">{t('cards.table.back')}</th>
            <th className="px-5 py-3 font-medium">{t('cards.table.tags')}</th>
            <th className="px-5 py-3 text-right font-medium">{t('cards.table.actions')}</th>
          </tr>
        </thead>
        <tbody>
          {cards.map((card, index) => (
            <tr key={card.id} className="border-t border-memorio-border">
              <td className="px-5 py-4 align-top">
                <div className="flex items-start gap-3">
                  <span className="mt-1.5 size-2 shrink-0 rounded-full" style={{ backgroundColor: accentForIndex(index) }} />
                  <span className="font-semibold text-memorio-text">{card.front}</span>
                </div>
              </td>
              <td className="px-5 py-4 align-top text-sm text-memorio-muted">{card.back}</td>
              <td className="px-5 py-4 align-top">
                <div className="flex flex-wrap gap-1.5">
                  {parseTags(card.tags).map((tag) => (
                    <Badge key={tag}>{tag}</Badge>
                  ))}
                </div>
              </td>
              <td className="px-5 py-4 align-top">
                <div className="flex items-center justify-end gap-2">
                  <button
                    type="button"
                    onClick={() => onEdit(card)}
                    title={t('cards.table.edit')}
                    className="flex size-8 items-center justify-center rounded-lg border border-memorio-border text-memorio-muted transition hover:border-memorio-primary hover:text-memorio-text"
                  >
                    ✎
                  </button>
                  <button
                    type="button"
                    onClick={() => onDelete(card)}
                    title={t('cards.table.delete')}
                    className="flex size-8 items-center justify-center rounded-lg border border-memorio-danger/40 text-memorio-danger transition hover:bg-memorio-danger/10"
                  >
                    ✕
                  </button>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
import { useTranslation } from 'react-i18next'
