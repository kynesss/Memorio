import type { PropsWithChildren } from 'react'
import { useEffect } from 'react'

interface ModalProps extends PropsWithChildren {
  onClose: () => void
  className?: string
}

export function Modal({ onClose, className = 'max-w-2xl', children }: ModalProps) {
  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === 'Escape') {
        onClose()
      }
    }

    window.addEventListener('keydown', handleKeyDown)
    document.body.style.overflow = 'hidden'

    return () => {
      window.removeEventListener('keydown', handleKeyDown)
      document.body.style.overflow = ''
    }
  }, [onClose])

  return (
    <div
      role="presentation"
      className="fixed inset-0 z-40 flex items-start justify-center overflow-y-auto bg-black/60 px-4 py-12 backdrop-blur-sm"
      onMouseDown={onClose}
    >
      <div
        role="dialog"
        aria-modal="true"
        className={`relative z-10 w-full overflow-hidden rounded-2xl border border-memorio-border bg-memorio-panel before:absolute before:inset-x-0 before:top-0 before:h-0.75 before:bg-memorio-primary before:content-[''] ${className}`}
        onMouseDown={(event) => event.stopPropagation()}
      >
        {children}
      </div>
    </div>
  )
}
