import type { PropsWithChildren } from 'react'
import { useEffect, useState } from 'react'
import { Navigate } from 'react-router-dom'
import { PageLoader } from '../components/common/Spinner'
import { refreshAccessToken } from './api'
import { tokenStore } from './tokenStore'

type AuthStatus = 'checking' | 'authenticated' | 'guest'

export function ProtectedRoute({ children }: PropsWithChildren) {
  const [status, setStatus] = useState<AuthStatus>(
    tokenStore.getAccessToken() ? 'authenticated' : 'checking',
  )

  useEffect(() => {
    if (status !== 'checking') {
      return
    }

    let active = true
    refreshAccessToken()
      .then(() => {
        if (active) {
          setStatus('authenticated')
        }
      })
      .catch(() => {
        if (active) {
          setStatus('guest')
        }
      })

    return () => {
      active = false
    }
  }, [status])

  if (status === 'checking') {
    return <PageLoader />
  }

  if (status === 'guest') {
    return <Navigate to="/login" replace />
  }

  return <>{children}</>
}
