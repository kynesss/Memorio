interface AuthHeaderProps {
  title: string
  subtitle?: string
}

export function AuthHeader({ title, subtitle }: AuthHeaderProps) {
  return (
    <header className="text-center">
      <div className="mx-auto flex size-11 items-center justify-center rounded-xl bg-memorio-primary text-2xl font-bold text-white">
        M
      </div>
      <h1 className="mt-4 text-[26px] font-bold leading-8 text-memorio-text">{title}</h1>
      {subtitle && <p className="mt-1 text-sm text-memorio-muted">{subtitle}</p>}
    </header>
  )
}
