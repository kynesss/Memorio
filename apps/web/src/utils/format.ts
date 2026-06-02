const accentColors = ['#29c7e0', '#3ecf8e', '#f5b60c', '#f5434c', '#8863f9'] as const

export function accentForIndex(index: number) {
  return accentColors[index % accentColors.length]
}

export function greetingKeyForHour(hour: number) {
  if (hour < 12) {
    return 'dashboard.greetings.morning'
  }
  if (hour < 18) {
    return 'dashboard.greetings.afternoon'
  }

  return 'dashboard.greetings.evening'
}

export function parseTags(tags: string | null) {
  if (!tags) {
    return []
  }

  return tags.split(/[,\n]/).map((tag) => tag.trim()).filter(Boolean)
}

export function serializeTags(tags: string[]) {
  return tags.join(', ')
}

export function buildCardSearchFilter(term: string) {
  const sanitized = term.trim().replace(/[(),|]/g, ' ').trim()
  return sanitized ? `(Front|Back)@=*${sanitized}` : undefined
}

export function formatRelativeDate(iso: string, language: string) {
  const date = new Date(iso)
  const diffMinutes = Math.round((Date.now() - date.getTime()) / 60000)
  const relativeTime = new Intl.RelativeTimeFormat(language, { numeric: 'auto', style: 'narrow' })

  if (diffMinutes < 1) {
    return relativeTime.format(0, 'minute')
  }
  if (diffMinutes < 60) {
    return relativeTime.format(-diffMinutes, 'minute')
  }

  const diffHours = Math.round(diffMinutes / 60)
  if (diffHours < 24) {
    return relativeTime.format(-diffHours, 'hour')
  }

  const diffDays = Math.round(diffHours / 24)
  if (diffDays < 7) {
    return relativeTime.format(-diffDays, 'day')
  }

  return date.toLocaleDateString(language, { month: 'short', day: 'numeric' })
}
