import type { PropsWithChildren } from 'react'
import { Navigate } from 'react-router-dom'
import { PageLoader } from '../components/common/Spinner'
import { useAuth } from './useAuth'

export function ProtectedRoute({ children }: PropsWithChildren) {
  const { status } = useAuth()

  if (status === 'checking') {
    return <PageLoader />
  }

  if (status === 'guest') {
    return <Navigate to="/login" replace />
  }

  return <>{children}</>
}
