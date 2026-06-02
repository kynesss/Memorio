export function displayNameFromEmail(email: string) {
  const localPart = email.split('@')[0] ?? ''
  const firstSegment = localPart.split(/[._-]/).filter(Boolean)[0] ?? localPart
  if (!firstSegment) {
    return 'there'
  }

  return firstSegment.charAt(0).toUpperCase() + firstSegment.slice(1)
}

export function initialsFromEmail(email: string) {
  const localPart = email.split('@')[0] ?? ''
  const segments = localPart.split(/[._-]/).filter(Boolean)
  const letters = segments.length > 1
    ? segments[0].charAt(0) + segments[1].charAt(0)
    : localPart.slice(0, 2)

  return letters.toUpperCase() || '?'
}
