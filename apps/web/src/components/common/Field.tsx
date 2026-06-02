import type { ComponentProps } from 'react'

const baseClasses =
  'w-full rounded-[10px] border bg-memorio-input px-4 text-sm text-memorio-text outline-none transition placeholder:text-memorio-subtle focus:border-memorio-primary'

interface TextFieldProps extends ComponentProps<'input'> {
  label: string
  error?: string
}

export function TextField({ label, error, className = '', ...props }: TextFieldProps) {
  return (
    <label className="block">
      <span className="mb-1.5 block text-xs font-medium text-memorio-muted">{label}</span>
      <input
        className={`${baseClasses} h-12 ${error ? 'border-memorio-danger' : 'border-memorio-border'} ${className}`}
        {...props}
      />
      {error && <span className="mt-1 block text-xs text-memorio-danger">{error}</span>}
    </label>
  )
}

interface TextAreaFieldProps extends ComponentProps<'textarea'> {
  label: string
  error?: string
}

export function TextAreaField({ label, error, className = '', ...props }: TextAreaFieldProps) {
  return (
    <label className="block">
      <span className="mb-1.5 block text-xs font-medium text-memorio-muted">{label}</span>
      <textarea
        className={`${baseClasses} min-h-28 resize-y py-3 ${error ? 'border-memorio-danger' : 'border-memorio-border'} ${className}`}
        {...props}
      />
      {error && <span className="mt-1 block text-xs text-memorio-danger">{error}</span>}
    </label>
  )
}
