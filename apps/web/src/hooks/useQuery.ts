import { useCallback, useEffect, useState } from 'react'
import { getApiErrorMessage } from '../auth/api'

export interface QueryState<T> {
  data: T | null
  isLoading: boolean
  error: string | null
  reload: () => void
}

export function useQuery<T>(fetcher: () => Promise<T>): QueryState<T> {
  const [data, setData] = useState<T | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [reloadToken, setReloadToken] = useState(0)

  const reload = useCallback(() => setReloadToken((token) => token + 1), [])

  useEffect(() => {
    let active = true

    const load = async () => {
      setIsLoading(true)

      try {
        const result = await fetcher()
        if (active) {
          setData(result)
          setError(null)
        }
      } catch (cause) {
        if (active) {
          setError(getApiErrorMessage(cause))
        }
      } finally {
        if (active) {
          setIsLoading(false)
        }
      }
    }

    void load()

    return () => {
      active = false
    }
  }, [fetcher, reloadToken])

  return { data, isLoading, error, reload }
}
