import { Button } from './Button'
import { Modal } from './Modal'

interface ConfirmDialogProps {
  title: string
  message: string
  confirmLabel?: string
  isProcessing?: boolean
  error?: string | null
  onConfirm: () => void
  onCancel: () => void
}

export function ConfirmDialog({
  title,
  message,
  confirmLabel = 'Delete',
  isProcessing = false,
  error,
  onConfirm,
  onCancel,
}: ConfirmDialogProps) {
  return (
    <Modal onClose={onCancel} className="max-w-md">
      <div className="p-6">
        <h2 className="text-lg font-bold text-memorio-text">{title}</h2>
        <p className="mt-2 text-sm text-memorio-muted">{message}</p>
        {error && <p className="mt-3 text-xs text-memorio-danger">{error}</p>}
        <div className="mt-6 flex justify-end gap-3">
          <Button variant="secondary" onClick={onCancel} disabled={isProcessing}>Cancel</Button>
          <Button variant="danger" onClick={onConfirm} disabled={isProcessing}>
            {isProcessing ? 'Deleting...' : confirmLabel}
          </Button>
        </div>
      </div>
    </Modal>
  )
}
