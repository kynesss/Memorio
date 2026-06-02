import { useTranslation } from 'react-i18next'
import type { StudyProgress } from '../../types/learning'

interface StudyHeaderProps {
  deckName: string
  progress: StudyProgress
  onExit: () => void
}

export function StudyHeader({ deckName, progress, onExit }: StudyHeaderProps) {
  const { t } = useTranslation()

  return (
    <div className="space-y-3">
      <div className="grid grid-cols-3 items-center gap-4">
        <button
          type="button"
          onClick={onExit}
          className="inline-flex h-9 items-center justify-self-start rounded-[10px] border border-memorio-border bg-memorio-panel px-4 text-sm text-memorio-muted transition hover:border-memorio-primary hover:text-memorio-text"
        >
          ← {t('study.exit')}
        </button>
        <h1 className="justify-self-center text-center text-base font-bold text-memorio-text">{deckName}</h1>
        <span className="inline-flex h-9 items-center justify-self-end rounded-[10px] border border-memorio-border bg-memorio-panel px-4 text-sm font-semibold text-memorio-text">
          {t('study.counter', { current: progress.current, total: progress.total })}
        </span>
      </div>

      <div className="h-1.5 w-full overflow-hidden rounded-full bg-memorio-input">
        <div
          className="h-full rounded-full bg-memorio-primary transition-all"
          style={{ width: `${progress.percent}%` }}
        />
      </div>

      <div className="flex justify-between text-xs text-memorio-subtle">
        <span>{t('study.progress.done', { count: progress.done })}</span>
        <span>{t('study.progress.remaining', { count: progress.remaining })}</span>
        <span>{t('study.progress.skipped', { count: progress.skipped })}</span>
      </div>
    </div>
  )
}
