import type { ComponentProps } from 'react'

interface FormFieldProps extends ComponentProps<'input'> {
  error?: string
  label: string
}

export function FormField({ error, label, ...inputProps }: FormFieldProps) {
  return (
    <label className="block">
      <span className="mb-1.25 block text-xs font-medium text-memorio-muted">{label}</span>
      <input
        className={`h-12 w-full rounded-[10px] border bg-memorio-input px-4 text-sm text-memorio-text outline-none transition placeholder:text-memorio-subtle focus:border-memorio-primary ${
          error ? 'border-memorio-danger' : 'border-memorio-border'
        }`}
        {...inputProps}
      />
      {error && <span className="mt-1 block text-xs text-memorio-danger">x&nbsp; {error}</span>}
    </label>
  )
}
