import { useEffect } from 'react'

interface ToastProps {
  message: string | null
  onClose: () => void
}

export function Toast({ message, onClose }: ToastProps) {
  useEffect(() => {
    if (!message) {
      return
    }

    const timeout = window.setTimeout(onClose, 4000)
    return () => window.clearTimeout(timeout)
  }, [message, onClose])

  if (!message) {
    return null
  }

  return (
    <div
      role="alert"
      className="fixed right-5 top-5 z-20 max-w-sm rounded-lg border border-memorio-danger/40 bg-[#27161e] px-4 py-3 text-sm text-memorio-danger shadow-xl"
    >
      {message}
    </div>
  )
}
