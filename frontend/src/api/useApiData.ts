import { useCallback, useEffect, useState } from 'react'

export function useApiData<T>(load: () => Promise<T>) {
  const [data, setData] = useState<T | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const reload = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      setData(await load())
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : 'Veriler yüklenemedi.')
    } finally {
      setLoading(false)
    }
  }, [load])

  useEffect(() => { void reload() }, [reload])
  return { data, setData, loading, error, reload }
}
