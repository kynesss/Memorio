import { zodResolver } from '@hookform/resolvers/zod'
import { useCallback, useState } from 'react'
import { useForm, useWatch } from 'react-hook-form'
import { useTranslation } from 'react-i18next'
import { Link, useNavigate } from 'react-router-dom'
import { z } from 'zod'
import { getApiErrorMessage, register as registerUser } from '../auth/api'
import { AuthCard } from '../components/auth/AuthCard'
import { AuthHeader } from '../components/auth/AuthHeader'
import { AuthLayout } from '../components/auth/AuthLayout'
import { FormField } from '../components/auth/FormField'
import { PasswordStrength } from '../components/auth/PasswordStrength'
import { Toast } from '../components/Toast'

type RegisterForm = {
  fullName: string
  email: string
  password: string
}

export function RegisterPage() {
  const { t } = useTranslation()
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
    resolver: zodResolver(z.object({
      fullName: z.string().trim().min(2, t('auth.register.fullNameRequired')),
      email: z.email(t('auth.validation.invalidEmail')),
      password: z.string().min(8, t('auth.register.passwordTooShort')),
    })),
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
        <AuthHeader title={t('auth.register.title')} subtitle={t('auth.register.subtitle')} />
        <form className="mt-3 border-t border-memorio-border pt-3.5" onSubmit={onSubmit} noValidate>
          <div className="space-y-1.5">
            <FormField
              label={t('auth.register.fullName')}
              type="text"
              placeholder={t('auth.register.fullNamePlaceholder')}
              autoComplete="name"
              error={errors.fullName?.message}
              {...register('fullName')}
            />
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
            {isSubmitting ? t('auth.register.creatingAccount') : t('auth.register.createAccount')}
          </button>
          <p className="pt-4 text-center text-[13px] text-memorio-muted">
            {t('auth.register.alreadyHaveAccount')}{' '}
            <Link className="font-semibold text-memorio-primary-light" to="/login">{t('auth.login.signIn')}</Link>
          </p>
        </form>
      </AuthCard>
    </AuthLayout>
  )
}
