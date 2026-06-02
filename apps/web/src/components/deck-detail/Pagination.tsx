interface PaginationProps {
  page: number
  totalPages: number
  onPageChange: (page: number) => void
}

function visiblePages(page: number, totalPages: number) {
  if (totalPages <= 7) {
    return Array.from({ length: totalPages }, (_, index) => index + 1)
  }

  const pages = new Set([1, totalPages, page, page - 1, page + 1])
  return [...pages].filter((value) => value >= 1 && value <= totalPages).sort((a, b) => a - b)
}

export function Pagination({ page, totalPages, onPageChange }: PaginationProps) {
  if (totalPages <= 1) {
    return null
  }

  const pages = visiblePages(page, totalPages)

  return (
    <div className="flex flex-wrap items-center gap-2">
      <button
        type="button"
        disabled={page <= 1}
        onClick={() => onPageChange(page - 1)}
        className="rounded-lg border border-memorio-border px-3 py-2 text-xs font-medium text-memorio-muted transition hover:text-memorio-text disabled:cursor-not-allowed disabled:opacity-40"
      >
        ← Prev
      </button>
      {pages.map((value, index) => {
        const previous = pages[index - 1]
        const hasGap = previous !== undefined && value - previous > 1
        return (
          <span key={value} className="flex items-center gap-2">
            {hasGap && <span className="text-memorio-subtle">…</span>}
            <button
              type="button"
              onClick={() => onPageChange(value)}
              className={`min-w-9 rounded-lg border px-3 py-2 text-xs font-medium transition ${
                value === page
                  ? 'border-memorio-primary bg-memorio-primary text-white'
                  : 'border-memorio-border text-memorio-muted hover:text-memorio-text'
              }`}
            >
              {value}
            </button>
          </span>
        )
      })}
      <button
        type="button"
        disabled={page >= totalPages}
        onClick={() => onPageChange(page + 1)}
        className="rounded-lg border border-memorio-border px-3 py-2 text-xs font-medium text-memorio-muted transition hover:text-memorio-text disabled:cursor-not-allowed disabled:opacity-40"
      >
        Next →
      </button>
    </div>
  )
}
