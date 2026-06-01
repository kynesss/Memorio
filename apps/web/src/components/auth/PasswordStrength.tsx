interface PasswordStrengthProps {
  password: string
}

export function PasswordStrength({ password }: PasswordStrengthProps) {
  const strength = getPasswordStrength(password)

  return (
    <div className="-mt-1">
      <div className="h-1 overflow-hidden rounded-sm bg-memorio-input">
        <div
          className={`h-full rounded-sm transition-all ${strength.color}`}
          style={{ width: strength.width }}
        />
      </div>
      <p className="mt-1.5 text-[11px] text-memorio-subtle">Password strength: {strength.label}</p>
    </div>
  )
}

function getPasswordStrength(password: string) {
  const checks = [
    password.length >= 8,
    /[a-z]/.test(password) && /[A-Z]/.test(password),
    /\d/.test(password),
    /[^a-zA-Z0-9]/.test(password),
  ]
  const score = checks.filter(Boolean).length

  if (score <= 1) {
    return { color: 'bg-memorio-danger', label: 'Weak', width: '30%' }
  }

  if (score <= 3) {
    return { color: 'bg-memorio-warning', label: 'Medium', width: '60%' }
  }

  return { color: 'bg-memorio-success', label: 'Strong', width: '100%' }
}
