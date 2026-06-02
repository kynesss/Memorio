import type { ComponentProps } from 'react'

type ButtonVariant = 'primary' | 'secondary' | 'danger' | 'ghost'

const variantClasses: Record<ButtonVariant, string> = {
  primary: 'bg-memorio-primary text-white hover:bg-memorio-primary-light',
  secondary: 'border border-memorio-border bg-memorio-input text-memorio-text hover:border-memorio-primary',
  danger: 'border border-memorio-danger/50 text-memorio-danger hover:bg-memorio-danger/10',
  ghost: 'text-memorio-muted hover:text-memorio-text',
}

interface ButtonProps extends ComponentProps<'button'> {
  variant?: ButtonVariant
}

export function Button({ variant = 'primary', className = '', type = 'button', ...props }: ButtonProps) {
  return (
    <button
      type={type}
      className={`inline-flex h-11 items-center justify-center gap-2 rounded-[10px] px-5 text-sm font-semibold transition disabled:cursor-not-allowed disabled:opacity-50 ${variantClasses[variant]} ${className}`}
      {...props}
    />
  )
}
