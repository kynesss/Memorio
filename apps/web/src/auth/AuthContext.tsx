import type { PropsWithChildren } from 'react'
import { createContext, useContext, useEffect, useMemo, useState } from 'react'
import { refreshAccessToken } from './api'
import { tokenStore } from './tokenStore'

export type AuthStatus = 'checking' | 'authenticated' | 'guest'

interface AuthContextValue {
  status: AuthStatus
  setAuthenticated: () => void
  setGuest: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

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

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }

  return context
}
