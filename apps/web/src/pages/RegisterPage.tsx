import { zodResolver } from '@hookform/resolvers/zod'
import { useCallback, useState } from 'react'
import { useForm, useWatch } from 'react-hook-form'
import { Link, useNavigate } from 'react-router-dom'
import { z } from 'zod'
import { getApiErrorMessage, register as registerUser } from '../auth/api'
import { AuthCard } from '../components/auth/AuthCard'
import { AuthHeader } from '../components/auth/AuthHeader'
import { AuthLayout } from '../components/auth/AuthLayout'
import { FormField } from '../components/auth/FormField'
import { PasswordStrength } from '../components/auth/PasswordStrength'
import { Toast } from '../components/Toast'

const registerSchema = z.object({
  fullName: z.string().trim().min(2, 'Full name is required'),
  email: z.email('Invalid email address'),
  password: z.string().min(8, 'Password must contain at least 8 characters'),
})

type RegisterForm = z.infer<typeof registerSchema>

export function RegisterPage() {
  const navigate = useNavigate()
  const [toast, setToast] = useState<string | null>(null)
  const closeToast = useCallback(() => setToast(null), [])
  const {
    formState: { errors, isSubmitting },
    control,
    handleSubmit,
    register,
  } = useForm<RegisterForm>({
    defaultValues: { password: '' },
    resolver: zodResolver(registerSchema),
  })
  const password = useWatch({ control, name: 'password' })

  const onSubmit = handleSubmit(async ({ email, password }) => {
    try {
      await registerUser(email, password)
      navigate('/dashboard', { replace: true })
    } catch (error) {
      setToast(getApiErrorMessage(error))
    }
  })

  return (
    <AuthLayout>
      <Toast message={toast} onClose={closeToast} />
      <AuthCard>
        <AuthHeader title="Create account" subtitle="Start learning smarter today" />
        <form className="mt-3 border-t border-memorio-border pt-3.5" onSubmit={onSubmit} noValidate>
          <div className="space-y-1.5">
            <FormField
              label="Full name"
              type="text"
              placeholder="Kamil Piróg"
              autoComplete="name"
              error={errors.fullName?.message}
              {...register('fullName')}
            />
            <FormField
              label="Email"
              type="email"
              placeholder="you@example.com"
              autoComplete="email"
              error={errors.email?.message}
              {...register('email')}
            />
            <FormField
              label="Password"
              type="password"
              placeholder="••••••••••••"
              autoComplete="new-password"
              error={errors.password?.message}
              {...register('password')}
            />
            <PasswordStrength password={password} />
          </div>
          <button
            className="mt-4 h-12 w-full rounded-[10px] bg-memorio-primary text-[15px] font-semibold text-white transition hover:bg-memorio-primary-light disabled:cursor-wait disabled:opacity-70"
            disabled={isSubmitting}
            type="submit"
          >
            {isSubmitting ? 'Creating account...' : 'Create account'}
          </button>
          <p className="pt-4 text-center text-[13px] text-memorio-muted">
            Already have an account?{' '}
            <Link className="font-semibold text-memorio-primary-light" to="/login">Sign in</Link>
          </p>
        </form>
      </AuthCard>
    </AuthLayout>
  )
}
