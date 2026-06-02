import type { ReactNode } from 'react'

interface EmptyStateProps {
  icon: string
  title: string
  description: string
  action?: ReactNode
  dashed?: boolean
}

export function EmptyState({ icon, title, description, action, dashed = false }: EmptyStateProps) {
  return (
    <div
      className={`flex flex-col items-center justify-center rounded-2xl px-6 py-14 text-center ${
        dashed ? 'border border-dashed border-memorio-border' : 'border border-memorio-border bg-memorio-panel'
      }`}
    >
      <span className="text-4xl">{icon}</span>
      <h3 className="mt-4 text-lg font-bold text-memorio-text">{title}</h3>
      <p className="mt-1 text-sm text-memorio-muted">{description}</p>
      {action && <div className="mt-6">{action}</div>}
    </div>
  )
}
