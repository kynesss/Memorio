import { useTranslation } from 'react-i18next'

export function Spinner({ className = '' }: { className?: string }) {
  const { t } = useTranslation()

  return (
    <span
      role="status"
      aria-label={t('common.loading')}
      className={`inline-block animate-spin rounded-full border-2 border-memorio-border border-t-memorio-primary ${className}`}
    />
  )
}

export function PageLoader() {
  return (
    <div className="flex min-h-[50vh] items-center justify-center">
      <Spinner className="size-8" />
    </div>
  )
}
