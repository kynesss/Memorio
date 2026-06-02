import { zodResolver } from '@hookform/resolvers/zod'
import { useForm } from 'react-hook-form'
import { useTranslation } from 'react-i18next'
import { z } from 'zod'
import type { Deck, DeckInput } from '../../types/flashcards'
import { Button } from '../common/Button'
import { TextAreaField, TextField } from '../common/Field'
import { Modal } from '../common/Modal'

type DeckFormValues = {
  name: string
  description?: string
}

interface DeckFormModalProps {
  deck?: Deck
  error?: string | null
  isSaving: boolean
  onSubmit: (input: DeckInput) => void
  onClose: () => void
}

export function DeckFormModal({ deck, error, isSaving, onSubmit, onClose }: DeckFormModalProps) {
  const { t } = useTranslation()
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<DeckFormValues>({
    resolver: zodResolver(z.object({
      name: z.string().trim().min(1, t('decks.form.nameRequired')).max(200, t('decks.form.nameTooLong')),
      description: z.string().trim().max(2000, t('decks.form.descriptionTooLong')).optional(),
    })),
    defaultValues: { name: deck?.name ?? '', description: deck?.description ?? '' },
  })

  const submit = handleSubmit((values) => {
    onSubmit({ name: values.name, description: values.description || undefined })
  })

  return (
    <Modal onClose={onClose} className="max-w-lg">
      <form className="p-6" onSubmit={submit} noValidate>
        <h2 className="text-lg font-bold text-memorio-text">{deck ? t('decks.form.editTitle') : t('decks.form.newTitle')}</h2>
        {error && (
          <p className="mt-3 rounded-lg border border-memorio-danger/40 bg-memorio-danger/10 px-3 py-2 text-xs text-memorio-danger">
            {error}
          </p>
        )}
        <div className="mt-5 space-y-4">
          <TextField label={t('decks.form.name')} placeholder={t('decks.form.namePlaceholder')} autoFocus error={errors.name?.message} {...register('name')} />
          <TextAreaField label={t('decks.form.description')} placeholder={t('decks.form.descriptionPlaceholder')} error={errors.description?.message} {...register('description')} />
        </div>
        <div className="mt-6 flex justify-end gap-3">
          <Button variant="secondary" onClick={onClose} disabled={isSaving}>{t('common.cancel')}</Button>
          <Button type="submit" disabled={isSaving}>
            {isSaving ? t('common.saving') : deck ? t('common.saveChanges') : t('decks.form.create')}
          </Button>
        </div>
      </form>
    </Modal>
  )
}
