import type { FormEvent, KeyboardEvent } from 'react'
import { useState } from 'react'
import type { Card, CardInput, CardType } from '../../types/flashcards'
import { parseTags, serializeTags } from '../../utils/format'
import { Button } from '../common/Button'
import { Modal } from '../common/Modal'

const cardTypes: { value: CardType; label: string }[] = [
  { value: 'Basic', label: '⇄ Basic' },
  { value: 'BasicReversed', label: '⇆ Basic + Reversed' },
  { value: 'Cloze', label: '{…} Cloze' },
]

const editorClasses =
  'w-full rounded-[10px] border border-memorio-border bg-memorio-input px-4 py-3 text-sm text-memorio-text outline-none transition placeholder:text-memorio-subtle focus:border-memorio-primary'

interface CardFormModalProps {
  card?: Card
  error?: string | null
  isSaving: boolean
  onSubmit: (input: CardInput) => void
  onClose: () => void
}

export function CardFormModal({ card, error, isSaving, onSubmit, onClose }: CardFormModalProps) {
  const [type, setType] = useState<CardType>(card?.type ?? 'Basic')
  const [front, setFront] = useState(card?.front ?? '')
  const [back, setBack] = useState(card?.back ?? '')
  const [tags, setTags] = useState<string[]>(parseTags(card?.tags ?? null))
  const [tagDraft, setTagDraft] = useState('')
  const [fieldError, setFieldError] = useState<string | null>(null)

  const commitTag = () => {
    const value = tagDraft.trim()
    if (value && !tags.includes(value)) {
      setTags([...tags, value])
    }
    setTagDraft('')
  }

  const handleTagKeyDown = (event: KeyboardEvent<HTMLInputElement>) => {
    if (event.key === 'Enter' || event.key === ',') {
      event.preventDefault()
      commitTag()
    } else if (event.key === 'Backspace' && !tagDraft && tags.length > 0) {
      setTags(tags.slice(0, -1))
    }
  }

  const submit = (event: FormEvent) => {
    event.preventDefault()
    if (!front.trim() || !back.trim()) {
      setFieldError('Front and back are required.')
      return
    }

    onSubmit({
      front: front.trim(),
      back: back.trim(),
      tags: tags.length > 0 ? serializeTags(tags) : undefined,
      type,
    })
  }

  return (
    <Modal onClose={onClose}>
      <form className="p-6" onSubmit={submit} noValidate>
        <div className="flex items-center gap-3">
          <h2 className="text-lg font-bold text-memorio-text">{card ? 'Edit flashcard' : 'Add flashcard'}</h2>
          {card && (
            <span className="rounded-md bg-memorio-primary/20 px-2 py-0.5 text-xs font-medium text-memorio-primary-light">Editing</span>
          )}
        </div>

        {(error ?? fieldError) && (
          <p className="mt-3 rounded-lg border border-memorio-danger/40 bg-memorio-danger/10 px-3 py-2 text-xs text-memorio-danger">
            {error ?? fieldError}
          </p>
        )}

        <div className="mt-5 space-y-4">
          <div>
            <span className="mb-1.5 block text-xs font-medium text-memorio-muted">Card type</span>
            <div className="grid grid-cols-3 gap-2 rounded-[10px] border border-memorio-border p-1">
              {cardTypes.map((option) => (
                <button
                  key={option.value}
                  type="button"
                  onClick={() => setType(option.value)}
                  className={`rounded-lg px-3 py-2 text-sm font-medium transition ${
                    type === option.value
                      ? 'bg-memorio-primary/20 text-memorio-primary-light'
                      : 'text-memorio-muted hover:text-memorio-text'
                  }`}
                >
                  {option.label}
                </button>
              ))}
            </div>
          </div>

          <label className="block">
            <span className="mb-1.5 block text-xs font-medium text-memorio-muted">Front</span>
            <textarea
              value={front}
              onChange={(event) => setFront(event.target.value)}
              placeholder="Term or question"
              autoFocus
              className={`${editorClasses} min-h-24 resize-y`}
            />
          </label>

          <label className="block">
            <span className="mb-1.5 block text-xs font-medium text-memorio-muted">Back</span>
            <textarea
              value={back}
              onChange={(event) => setBack(event.target.value)}
              placeholder="Definition or answer"
              className={`${editorClasses} min-h-24 resize-y`}
            />
          </label>

          <div>
            <span className="mb-1.5 block text-xs font-medium text-memorio-muted">Tags</span>
            <div className="flex flex-wrap items-center gap-2 rounded-[10px] border border-memorio-border bg-memorio-input px-3 py-2">
              {tags.map((tag) => (
                <span key={tag} className="inline-flex items-center gap-1 rounded-md bg-memorio-primary/15 px-2 py-0.5 text-xs text-memorio-primary-light">
                  {tag}
                  <button type="button" onClick={() => setTags(tags.filter((value) => value !== tag))} className="text-memorio-primary-light/70 hover:text-memorio-primary-light">
                    ×
                  </button>
                </span>
              ))}
              <input
                value={tagDraft}
                onChange={(event) => setTagDraft(event.target.value)}
                onKeyDown={handleTagKeyDown}
                onBlur={commitTag}
                placeholder="Add tags..."
                className="min-w-32 flex-1 bg-transparent text-sm text-memorio-text outline-none placeholder:text-memorio-subtle"
              />
            </div>
          </div>
        </div>

        <div className="mt-6 flex justify-end gap-3">
          <Button variant="secondary" onClick={onClose} disabled={isSaving}>Cancel</Button>
          <Button type="submit" disabled={isSaving}>
            {isSaving ? 'Saving...' : card ? 'Save changes' : 'Add card'}
          </Button>
        </div>
      </form>
    </Modal>
  )
}
