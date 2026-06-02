import { Button } from '../common/Button'

const filters = ['All', 'Due', 'New', 'Hard', 'Mastered'] as const

interface CardsToolbarProps {
  search: string
  onSearchChange: (value: string) => void
  onAddCard: () => void
}

export function CardsToolbar({ search, onSearchChange, onAddCard }: CardsToolbarProps) {
  return (
    <div className="flex flex-wrap items-center gap-4">
      <div className="relative min-w-64 flex-1">
        <span className="pointer-events-none absolute left-3.5 top-1/2 -translate-y-1/2 text-sm text-memorio-subtle">🔍</span>
        <input
          value={search}
          onChange={(event) => onSearchChange(event.target.value)}
          placeholder="Search flashcards..."
          className="h-11 w-full rounded-[10px] border border-memorio-border bg-memorio-panel pl-10 pr-4 text-sm text-memorio-text outline-none transition placeholder:text-memorio-subtle focus:border-memorio-primary"
        />
      </div>
      <div className="flex items-center gap-2">
        {filters.map((filter) => {
          const isActive = filter === 'All'
          return (
            <button
              key={filter}
              type="button"
              disabled={!isActive}
              title={isActive ? undefined : 'Available once study tracking is live'}
              className={`rounded-full border px-3.5 py-1.5 text-xs font-medium transition ${
                isActive
                  ? 'border-memorio-primary text-memorio-primary-light'
                  : 'cursor-not-allowed border-memorio-border text-memorio-subtle'
              }`}
            >
              {filter}
            </button>
          )
        })}
      </div>
      <Button className="ml-auto" onClick={onAddCard}>+ Add Card</Button>
    </div>
  )
}
