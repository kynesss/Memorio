import { zodResolver } from '@hookform/resolvers/zod'
import { useCallback, useState } from 'react'
import { useForm } from 'react-hook-form'
import { Link, useNavigate } from 'react-router-dom'
import { z } from 'zod'
import { getApiErrorMessage, login } from '../auth/api'
import { AuthCard } from '../components/auth/AuthCard'
import { AuthHeader } from '../components/auth/AuthHeader'
import { AuthLayout } from '../components/auth/AuthLayout'
import { FormField } from '../components/auth/FormField'
import { Toast } from '../components/Toast'

const loginSchema = z.object({
  email: z.email('Invalid email address'),
  password: z.string().min(1, 'Password is required'),
})

type LoginForm = z.infer<typeof loginSchema>

export function LoginPage() {
  const navigate = useNavigate()
  const [apiError, setApiError] = useState<string | null>(null)
  const [toast, setToast] = useState<string | null>(null)
  const closeToast = useCallback(() => setToast(null), [])
  const {
    formState: { errors, isSubmitting },
    handleSubmit,
    register,
  } = useForm<LoginForm>({
    resolver: zodResolver(loginSchema),
  })

  const onSubmit = handleSubmit(async ({ email, password }) => {
    setApiError(null)

    try {
      await login(email, password)
      navigate('/dashboard', { replace: true })
    } catch (error) {
      const message = getApiErrorMessage(error)
      setApiError(message)
      setToast(message)
    }
  })

  return (
    <AuthLayout>
      <Toast message={toast} onClose={closeToast} />
      <AuthCard hasError={Boolean(apiError)}>
        <AuthHeader title="Welcome back" subtitle="Sign in to continue learning" />
        <form className="mt-3 border-t border-memorio-border pt-3.5" onSubmit={onSubmit} noValidate>
          {apiError && (
            <div className="mb-3 rounded-lg border border-memorio-danger/40 bg-memorio-danger/10 px-3 py-2.5 text-xs font-medium text-memorio-danger">
              x&nbsp; {apiError}
            </div>
          )}
          <div className="space-y-1.5">
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
              autoComplete="current-password"
              error={errors.password?.message}
              {...register('password')}
            />
          </div>
          <button className="ml-auto mt-1.5 block text-xs font-medium text-memorio-primary-light" type="button">
            Forgot password?
          </button>
          <button
            className="mt-1.5 h-12 w-full rounded-[10px] bg-memorio-primary text-[15px] font-semibold text-white transition hover:bg-memorio-primary-light disabled:cursor-wait disabled:opacity-70"
            disabled={isSubmitting}
            type="submit"
          >
            {isSubmitting ? 'Signing in...' : 'Sign in'}
          </button>
          <div className="my-3 flex items-center gap-3 text-xs text-memorio-subtle">
            <span className="h-px flex-1 bg-memorio-border" />
            or
            <span className="h-px flex-1 bg-memorio-border" />
          </div>
          <p className="pt-2 text-center text-[13px] text-memorio-muted">
            Don&apos;t have an account?{' '}
            <Link className="font-semibold text-memorio-primary-light" to="/register">Sign up</Link>
          </p>
        </form>
      </AuthCard>
    </AuthLayout>
  )
}
