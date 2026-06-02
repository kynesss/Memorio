interface Stat {
  icon: string
  label: string
  value: string
  accent: string
}

const stats: Stat[] = [
  { icon: '🔥', label: 'Day streak', value: '0', accent: '#f5b60c' },
  { icon: '📚', label: 'Due today', value: '0', accent: '#f5434c' },
  { icon: '🗂', label: 'Total cards', value: '0', accent: '#29c7e0' },
  { icon: '✓', label: 'Mastered', value: '0%', accent: '#3ecf8e' },
  { icon: '⏱', label: 'Study time today', value: '0m', accent: '#8863f9' },
]

export function StatsRow() {
  return (
    <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-5">
      {stats.map((stat) => (
        <div
          key={stat.label}
          className="rounded-xl border border-memorio-border bg-memorio-panel px-5 py-4"
          style={{ borderTopColor: stat.accent, borderTopWidth: 2 }}
        >
          <p className="text-2xl font-bold" style={{ color: stat.accent }}>{stat.value}</p>
          <p className="mt-1 text-xs text-memorio-muted">{stat.icon} {stat.label}</p>
        </div>
      ))}
    </div>
  )
}
