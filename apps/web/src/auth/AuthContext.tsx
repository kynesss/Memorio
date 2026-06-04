import type { PropsWithChildren } from 'react'
import { useEffect, useMemo, useState } from 'react'
import { refreshAccessToken } from './api'
import { AuthContext, type AuthContextValue, type AuthStatus } from './auth-context'
import { tokenStore } from './tokenStore'

export function AuthProvider({ children }: PropsWithChildren) {
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

  const value = useMemo<AuthContextValue>(() => ({
    status,
    setAuthenticated: () => setStatus('authenticated'),
    setGuest: () => setStatus('guest'),
  }), [status])

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
