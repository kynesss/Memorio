import { useTranslation } from 'react-i18next'

interface Stat {
  icon: string
  labelKey: string
  value: string
  valueKey?: string
  accent: string
}

const stats: Stat[] = [
  { icon: '🔥', labelKey: 'dashboard.stats.dayStreak', value: '0', accent: '#f5b60c' },
  { icon: '📚', labelKey: 'dashboard.stats.dueToday', value: '0', accent: '#f5434c' },
  { icon: '🗂', labelKey: 'dashboard.stats.totalCards', value: '0', accent: '#29c7e0' },
  { icon: '✓', labelKey: 'dashboard.stats.mastered', value: '0%', accent: '#3ecf8e' },
  { icon: '⏱', labelKey: 'dashboard.stats.studyTimeToday', value: '', valueKey: 'dashboard.stats.noStudyTime', accent: '#8863f9' },
]

export function StatsRow() {
  const { t } = useTranslation()

  return (
    <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-5">
      {stats.map((stat) => (
        <div
          key={stat.labelKey}
          className="rounded-xl border border-memorio-border bg-memorio-panel px-5 py-4"
          style={{ borderTopColor: stat.accent, borderTopWidth: 2 }}
        >
          <p className="text-2xl font-bold" style={{ color: stat.accent }}>{stat.valueKey ? t(stat.valueKey) : stat.value}</p>
          <p className="mt-1 text-xs text-memorio-muted">{stat.icon} {t(stat.labelKey)}</p>
        </div>
      ))}
    </div>
  )
}
