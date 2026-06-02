import { useCallback } from 'react'
import { getCurrentUser } from '../api/user'
import { useQuery } from './useQuery'

export function useCurrentUser() {
  const fetcher = useCallback(() => getCurrentUser(), [])
  return useQuery(fetcher)
}
