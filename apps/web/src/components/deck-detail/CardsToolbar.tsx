import { useTranslation } from 'react-i18next'
import { Button } from '../common/Button'

const filters = ['all', 'due', 'new', 'hard', 'mastered'] as const

interface CardsToolbarProps {
  search: string
  onSearchChange: (value: string) => void
  onAddCard: () => void
}

export function CardsToolbar({ search, onSearchChange, onAddCard }: CardsToolbarProps) {
  const { t } = useTranslation()

  return (
    <div className="flex flex-wrap items-center gap-4">
      <div className="relative min-w-64 flex-1">
        <span className="pointer-events-none absolute left-3.5 top-1/2 -translate-y-1/2 text-sm text-memorio-subtle">🔍</span>
        <input
          value={search}
          onChange={(event) => onSearchChange(event.target.value)}
          placeholder={t('cards.searchPlaceholder')}
          className="h-11 w-full rounded-[10px] border border-memorio-border bg-memorio-panel pl-10 pr-4 text-sm text-memorio-text outline-none transition placeholder:text-memorio-subtle focus:border-memorio-primary"
        />
      </div>
      <div className="flex items-center gap-2">
        {filters.map((filter) => {
          const isActive = filter === 'all'
          return (
            <button
              key={filter}
              type="button"
              disabled={!isActive}
              title={isActive ? undefined : t('common.availableWhenStudyTrackingIsLive')}
              className={`rounded-full border px-3.5 py-1.5 text-xs font-medium transition ${
                isActive
                  ? 'border-memorio-primary text-memorio-primary-light'
                  : 'cursor-not-allowed border-memorio-border text-memorio-subtle'
              }`}
            >
              {t(`cards.filters.${filter}`)}
            </button>
          )
        })}
      </div>
      <Button className="ml-auto" onClick={onAddCard}>{t('cards.add')}</Button>
    </div>
  )
}
