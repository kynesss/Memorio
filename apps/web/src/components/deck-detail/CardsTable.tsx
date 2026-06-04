import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import type { Card } from '../../types/flashcards'
import { getCardImages, parseCardContent } from '../../utils/cardContent'
import { accentForIndex, parseTags } from '../../utils/format'
import { Badge } from '../common/Badge'
import { Modal } from '../common/Modal'

interface CardsTableProps {
  cards: Card[]
  onEdit: (card: Card) => void
  onDelete: (card: Card) => void
}

export function CardsTable({ cards, onEdit, onDelete }: CardsTableProps) {
  const { t } = useTranslation()
  const [preview, setPreview] = useState<{ src: string; fileName: string } | null>(null)

  return (
    <>
      <div className="overflow-x-auto rounded-xl border border-memorio-border">
        <table className="w-full border-collapse text-left">
          <thead>
            <tr className="bg-memorio-panel text-xs uppercase tracking-wide text-memorio-subtle">
              <th className="px-5 py-3 font-medium">{t('cards.table.front')}</th>
              <th className="px-5 py-3 font-medium">{t('cards.table.back')}</th>
              <th className="px-5 py-3 font-medium">{t('cards.table.media')}</th>
              <th className="px-5 py-3 font-medium">{t('cards.table.tags')}</th>
              <th className="px-5 py-3 text-right font-medium">{t('cards.table.actions')}</th>
            </tr>
          </thead>
          <tbody>
            {cards.map((card, index) => {
              const front = parseCardContent(card.front, card.mediaItems)
              const back = parseCardContent(card.back, card.mediaItems)
              const images = getCardImages(card)

              return (
                <tr key={card.id} className="border-t border-memorio-border">
                  <td className="px-5 py-4 align-top">
                    <div className="flex items-start gap-3">
                      <span className="mt-1.5 size-2 shrink-0 rounded-full" style={{ backgroundColor: accentForIndex(index) }} />
                      <span className="font-semibold text-memorio-text">{front.text}</span>
                    </div>
                  </td>
                  <td className="px-5 py-4 align-top text-sm text-memorio-muted">{back.text}</td>
                  <td className="px-5 py-4 align-top">
                    {images.length > 0 ? (
                      <div className="flex -space-x-2">
                        {images.slice(0, 3).map((image) => (
                          <button
                            key={image.id}
                            type="button"
                            onClick={() => setPreview({ src: image.src, fileName: image.fileName })}
                            title={image.fileName}
                            className="relative rounded-lg transition hover:z-10 hover:scale-105"
                          >
                            <img
                              src={image.src}
                              alt={image.fileName}
                              className="size-9 rounded-lg border border-memorio-border bg-memorio-input object-cover"
                            />
                          </button>
                        ))}
                        {images.length > 3 && (
                          <span className="flex size-9 items-center justify-center rounded-lg border border-memorio-border bg-memorio-input text-xs font-semibold text-memorio-muted">
                            +{images.length - 3}
                          </span>
                        )}
                      </div>
                    ) : (
                      <span className="text-sm text-memorio-subtle">-</span>
                    )}
                  </td>
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
              )
            })}
          </tbody>
        </table>
      </div>

      {preview && (
        <Modal onClose={() => setPreview(null)} className="max-w-5xl">
          <div className="bg-memorio-bg p-4">
            <img src={preview.src} alt={preview.fileName} className="max-h-[80vh] w-full rounded-xl object-contain" />
          </div>
        </Modal>
      )}
    </>
  )
}
