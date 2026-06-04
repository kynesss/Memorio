import { createContext } from 'react'

export type AuthStatus = 'checking' | 'authenticated' | 'guest'

export interface AuthContextValue {
  status: AuthStatus
  setAuthenticated: () => void
  setGuest: () => void
}

export const AuthContext = createContext<AuthContextValue | null>(null)
