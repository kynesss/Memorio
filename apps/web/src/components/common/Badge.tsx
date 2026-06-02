import type { PropsWithChildren } from 'react'

interface BadgeProps extends PropsWithChildren {
  className?: string
}

export function Badge({ children, className = '' }: BadgeProps) {
  return (
    <span className={`inline-flex items-center rounded-md bg-memorio-input px-2 py-0.5 text-xs text-memorio-muted ${className}`}>
      {children}
    </span>
  )
}
