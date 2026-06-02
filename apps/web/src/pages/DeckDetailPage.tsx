import { useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { createCard, deleteCard, updateCard } from '../api/cards'
import { deleteDeck, updateDeck } from '../api/decks'
import { getApiErrorMessage } from '../auth/api'
import { Button } from '../components/common/Button'
import { ConfirmDialog } from '../components/common/ConfirmDialog'
import { EmptyState } from '../components/common/EmptyState'
import { PageLoader } from '../components/common/Spinner'
import { CardFormModal } from '../components/deck-detail/CardFormModal'
import { CardsTable } from '../components/deck-detail/CardsTable'
import { CardsToolbar } from '../components/deck-detail/CardsToolbar'
import { DeckDetailHeader } from '../components/deck-detail/DeckDetailHeader'
import { Pagination } from '../components/deck-detail/Pagination'
import { DeckFormModal } from '../components/decks/DeckFormModal'
import { AppLayout } from '../components/layout/AppLayout'
import { useCardCount, useCards } from '../hooks/useCards'
import { useCurrentUser } from '../hooks/useCurrentUser'
import { useDeck } from '../hooks/useDeck'
import { useDebouncedValue } from '../hooks/useDebouncedValue'
import type { Card, CardInput, DeckInput } from '../types/flashcards'

type CardModalState = { card: Card | null } | null

export function DeckDetailPage() {
  const { deckId = '' } = useParams()
  const navigate = useNavigate()

  const user = useCurrentUser()
  const deckQuery = useDeck(deckId)
  const cardCountQuery = useCardCount(deckId)

  const [search, setSearch] = useState('')
  const debouncedSearch = useDebouncedValue(search)
  const [page, setPage] = useState(1)
  const cardsQuery = useCards(deckId, page, debouncedSearch)

  const [isEditingDeck, setIsEditingDeck] = useState(false)
  const [isDeletingDeckOpen, setIsDeletingDeckOpen] = useState(false)
  const [deckActionError, setDeckActionError] = useState<string | null>(null)
  const [isDeckSaving, setIsDeckSaving] = useState(false)
  const [isDeckDeleting, setIsDeckDeleting] = useState(false)

  const [cardModal, setCardModal] = useState<CardModalState>(null)
  const [cardToDelete, setCardToDelete] = useState<Card | null>(null)
  const [cardActionError, setCardActionError] = useState<string | null>(null)
  const [isCardSaving, setIsCardSaving] = useState(false)
  const [isCardDeleting, setIsCardDeleting] = useState(false)

  const totalPages = cardsQuery.data?.totalPages ?? 0
  const email = user.data?.email ?? ''
  const cards = cardsQuery.data?.items ?? []
  const filteredTotal = cardsQuery.data?.totalCount ?? 0

  const reloadCards = () => {
    cardsQuery.reload()
    cardCountQuery.reload()
  }

  const handleSearchChange = (value: string) => {
    setSearch(value)
    setPage(1)
  }

  const handleDeckUpdate = async (input: DeckInput) => {
    setIsDeckSaving(true)
    setDeckActionError(null)

    try {
      await updateDeck(deckId, input)
      setIsEditingDeck(false)
      deckQuery.reload()
    } catch (error) {
      setDeckActionError(getApiErrorMessage(error))
    } finally {
      setIsDeckSaving(false)
    }
  }

  const handleDeckDelete = async () => {
    setIsDeckDeleting(true)
    setDeckActionError(null)

    try {
      await deleteDeck(deckId)
      navigate('/dashboard', { replace: true })
    } catch (error) {
      setDeckActionError(getApiErrorMessage(error))
      setIsDeckDeleting(false)
    }
  }

  const handleCardSubmit = async (input: CardInput) => {
    if (!cardModal) {
      return
    }

    setIsCardSaving(true)
    setCardActionError(null)

    try {
      if (cardModal.card) {
        await updateCard(deckId, cardModal.card.id, input)
      } else {
        await createCard(deckId, input)
      }
      setCardModal(null)
      reloadCards()
    } catch (error) {
      setCardActionError(getApiErrorMessage(error))
    } finally {
      setIsCardSaving(false)
    }
  }

  const handleCardDelete = async () => {
    if (!cardToDelete) {
      return
    }

    setIsCardDeleting(true)
    setCardActionError(null)
    const isLastOnPage = cards.length === 1 && page > 1

    try {
      await deleteCard(deckId, cardToDelete.id)
      setCardToDelete(null)
      if (isLastOnPage) {
        setPage(page - 1)
      }
      reloadCards()
    } catch (error) {
      setCardActionError(getApiErrorMessage(error))
    } finally {
      setIsCardDeleting(false)
    }
  }

  return (
    <AppLayout email={email}>
      {deckQuery.isLoading ? (
        <PageLoader />
      ) : deckQuery.error || !deckQuery.data ? (
        <EmptyState
          icon="🔍"
          title="Deck not found"
          description={deckQuery.error ?? 'This deck no longer exists.'}
          action={<Button onClick={() => navigate('/dashboard')}>Back to dashboard</Button>}
        />
      ) : (
        <div className="space-y-8">
          <nav className="flex items-center gap-2 text-sm text-memorio-muted">
            <Link to="/dashboard" className="text-memorio-primary-light hover:underline">My Decks</Link>
            <span className="text-memorio-subtle">/</span>
            <span className="text-memorio-text">{deckQuery.data.name}</span>
          </nav>

          <DeckDetailHeader
            deck={deckQuery.data}
            cardCount={cardCountQuery.data ?? 0}
            onEdit={() => {
              setDeckActionError(null)
              setIsEditingDeck(true)
            }}
            onDelete={() => {
              setDeckActionError(null)
              setIsDeletingDeckOpen(true)
            }}
          />

          <div className="space-y-5">
            <CardsToolbar
              search={search}
              onSearchChange={handleSearchChange}
              onAddCard={() => {
                setCardActionError(null)
                setCardModal({ card: null })
              }}
            />

            {cardsQuery.isLoading ? (
              <PageLoader />
            ) : cardsQuery.error ? (
              <p className="text-sm text-memorio-danger">{cardsQuery.error}</p>
            ) : cards.length === 0 ? (
              debouncedSearch ? (
                <EmptyState
                  icon="🔍"
                  title="No matching flashcards"
                  description="Try a different search term."
                  action={<Button variant="secondary" onClick={() => setSearch('')}>Clear search</Button>}
                />
              ) : (
                <EmptyState
                  dashed
                  icon="🗂"
                  title="No flashcards yet"
                  description="Add your first flashcard to start learning."
                  action={<Button onClick={() => setCardModal({ card: null })}>+ Add First Card</Button>}
                />
              )
            ) : (
              <div className="space-y-4">
                <CardsTable
                  cards={cards}
                  onEdit={(card) => {
                    setCardActionError(null)
                    setCardModal({ card })
                  }}
                  onDelete={(card) => {
                    setCardActionError(null)
                    setCardToDelete(card)
                  }}
                />
                <div className="flex flex-wrap items-center justify-between gap-4">
                  <span className="text-sm text-memorio-subtle">
                    Showing {cards.length} of {filteredTotal} cards
                  </span>
                  <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
                </div>
              </div>
            )}
          </div>
        </div>
      )}

      {isEditingDeck && deckQuery.data && (
        <DeckFormModal
          deck={deckQuery.data}
          isSaving={isDeckSaving}
          error={deckActionError}
          onSubmit={handleDeckUpdate}
          onClose={() => setIsEditingDeck(false)}
        />
      )}

      {isDeletingDeckOpen && (
        <ConfirmDialog
          title="Delete deck"
          message="This permanently deletes the deck and all its flashcards. This cannot be undone."
          confirmLabel="Delete deck"
          isProcessing={isDeckDeleting}
          error={deckActionError}
          onConfirm={handleDeckDelete}
          onCancel={() => setIsDeletingDeckOpen(false)}
        />
      )}

      {cardModal && (
        <CardFormModal
          card={cardModal.card ?? undefined}
          isSaving={isCardSaving}
          error={cardActionError}
          onSubmit={handleCardSubmit}
          onClose={() => setCardModal(null)}
        />
      )}

      {cardToDelete && (
        <ConfirmDialog
          title="Delete flashcard"
          message="This permanently deletes this flashcard. This cannot be undone."
          isProcessing={isCardDeleting}
          error={cardActionError}
          onConfirm={handleCardDelete}
          onCancel={() => setCardToDelete(null)}
        />
      )}
    </AppLayout>
  )
}
