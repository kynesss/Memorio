import type { PropsWithChildren } from 'react'

interface AuthCardProps extends PropsWithChildren {
  hasError?: boolean
}

export function AuthCard({ children, hasError = false }: AuthCardProps) {
  return (
    <section
      className={`relative z-10 w-full max-w-110 overflow-hidden rounded-3xl border border-memorio-border bg-memorio-panel px-10 pb-8 pt-8 ${
        hasError ? 'before:bg-memorio-danger' : 'before:bg-memorio-primary'
      } before:absolute before:inset-x-0 before:top-0 before:h-0.75 before:content-['']`}
    >
      {children}
    </section>
  )
}
