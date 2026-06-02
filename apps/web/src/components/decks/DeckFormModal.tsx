import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { z } from 'zod'
import type { Deck, DeckInput } from '../../types/flashcards'
import { Button } from '../common/Button'
import { TextAreaField, TextField } from '../common/Field'
import { Modal } from '../common/Modal'

const deckSchema = z.object({
  name: z.string().trim().min(1, 'Name is required').max(200, 'Name is too long'),
  description: z.string().trim().max(2000, 'Description is too long').optional(),
})

type DeckFormValues = z.infer<typeof deckSchema>

interface DeckFormModalProps {
  deck?: Deck
  error?: string | null
  isSaving: boolean
  onSubmit: (input: DeckInput) => void
  onClose: () => void
}

export function DeckFormModal({ deck, error, isSaving, onSubmit, onClose }: DeckFormModalProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<DeckFormValues>({
    resolver: zodResolver(deckSchema),
    defaultValues: { name: deck?.name ?? '', description: deck?.description ?? '' },
  })

  const submit = handleSubmit((values) => {
    onSubmit({ name: values.name, description: values.description || undefined })
  })

  return (
    <Modal onClose={onClose} className="max-w-lg">
      <form className="p-6" onSubmit={submit} noValidate>
        <h2 className="text-lg font-bold text-memorio-text">{deck ? 'Edit deck' : 'New deck'}</h2>
        {error && (
          <p className="mt-3 rounded-lg border border-memorio-danger/40 bg-memorio-danger/10 px-3 py-2 text-xs text-memorio-danger">
            {error}
          </p>
        )}
        <div className="mt-5 space-y-4">
          <TextField label="Name" placeholder="Spanish Vocabulary" autoFocus error={errors.name?.message} {...register('name')} />
          <TextAreaField label="Description" placeholder="What is this deck about?" error={errors.description?.message} {...register('description')} />
        </div>
        <div className="mt-6 flex justify-end gap-3">
          <Button variant="secondary" onClick={onClose} disabled={isSaving}>Cancel</Button>
          <Button type="submit" disabled={isSaving}>
            {isSaving ? 'Saving...' : deck ? 'Save changes' : 'Create deck'}
          </Button>
        </div>
      </form>
    </Modal>
  )
}
