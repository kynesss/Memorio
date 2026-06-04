import { zodResolver } from '@hookform/resolvers/zod'
import { useCallback, useState } from 'react'
import { useForm } from 'react-hook-form'
import { useTranslation } from 'react-i18next'
import { Link, useNavigate } from 'react-router-dom'
import { z } from 'zod'
import { getApiErrorMessage, login } from '../auth/api'
import { useAuth } from '../auth/useAuth'
import { AuthCard } from '../components/auth/AuthCard'
import { AuthHeader } from '../components/auth/AuthHeader'
import { AuthLayout } from '../components/auth/AuthLayout'
import { FormField } from '../components/auth/FormField'
import { Toast } from '../components/Toast'

type LoginForm = {
  email: string
  password: string
}

export function LoginPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const { setAuthenticated } = useAuth()
  const [apiError, setApiError] = useState<string | null>(null)
  const [toast, setToast] = useState<string | null>(null)
  const closeToast = useCallback(() => setToast(null), [])
  const {
    formState: { errors, isSubmitting },
    handleSubmit,
    register,
  } = useForm<LoginForm>({
    resolver: zodResolver(z.object({
      email: z.email(t('auth.validation.invalidEmail')),
      password: z.string().min(1, t('auth.login.passwordRequired')),
    })),
  })

  const onSubmit = handleSubmit(async ({ email, password }) => {
    setApiError(null)

    try {
      await login(email, password)
      setAuthenticated()
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
        <AuthHeader title={t('auth.login.title')} subtitle={t('auth.login.subtitle')} />
        <form className="mt-3 border-t border-memorio-border pt-3.5" onSubmit={onSubmit} noValidate>
          {apiError && (
            <div className="mb-3 rounded-lg border border-memorio-danger/40 bg-memorio-danger/10 px-3 py-2.5 text-xs font-medium text-memorio-danger">
              x&nbsp; {apiError}
            </div>
          )}
          <div className="space-y-1.5">
            <FormField
              label={t('auth.email')}
              type="email"
              placeholder={t('auth.emailPlaceholder')}
              autoComplete="email"
              error={errors.email?.message}
              {...register('email')}
            />
            <FormField
              label={t('auth.password')}
              type="password"
              placeholder="••••••••••••"
              autoComplete="current-password"
              error={errors.password?.message}
              {...register('password')}
            />
          </div>
          <button className="ml-auto mt-1.5 block text-xs font-medium text-memorio-primary-light" type="button">
            {t('auth.login.forgotPassword')}
          </button>
          <button
            className="mt-1.5 h-12 w-full rounded-[10px] bg-memorio-primary text-[15px] font-semibold text-white transition hover:bg-memorio-primary-light disabled:cursor-wait disabled:opacity-70"
            disabled={isSubmitting}
            type="submit"
          >
            {isSubmitting ? t('auth.login.signingIn') : t('auth.login.signIn')}
          </button>
          <div className="my-3 flex items-center gap-3 text-xs text-memorio-subtle">
            <span className="h-px flex-1 bg-memorio-border" />
            {t('auth.login.or')}
            <span className="h-px flex-1 bg-memorio-border" />
          </div>
          <p className="pt-2 text-center text-[13px] text-memorio-muted">
            {t('auth.login.noAccount')}{' '}
            <Link className="font-semibold text-memorio-primary-light" to="/register">{t('auth.login.signUp')}</Link>
          </p>
        </form>
      </AuthCard>
    </AuthLayout>
  )
}
