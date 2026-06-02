import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { createDeck } from '../api/decks'
import { getApiErrorMessage } from '../auth/api'
import { Button } from '../components/common/Button'
import { EmptyState } from '../components/common/EmptyState'
import { PageLoader } from '../components/common/Spinner'
import { DeckCard } from '../components/dashboard/DeckCard'
import { FeaturedDeck } from '../components/dashboard/FeaturedDeck'
import { HeroBanner } from '../components/dashboard/HeroBanner'
import { StatsRow } from '../components/dashboard/StatsRow'
import { DeckFormModal } from '../components/decks/DeckFormModal'
import { AppLayout } from '../components/layout/AppLayout'
import { useCurrentUser } from '../hooks/useCurrentUser'
import { useDecks } from '../hooks/useDecks'
import type { DeckInput } from '../types/flashcards'
import { accentForIndex, greetingKeyForHour } from '../utils/format'
import { displayNameFromEmail } from '../utils/user'

export function DashboardPage() {
  const { t } = useTranslation()
  const user = useCurrentUser()
  const decksQuery = useDecks()
  const [isCreating, setIsCreating] = useState(false)
  const [isSaving, setIsSaving] = useState(false)
  const [saveError, setSaveError] = useState<string | null>(null)

  const email = user.data?.email ?? ''
  const decks = decksQuery.data?.items ?? []
  const [featured, ...rest] = decks

  const openCreate = () => {
    setSaveError(null)
    setIsCreating(true)
  }

  const handleCreate = async (input: DeckInput) => {
    setIsSaving(true)
    setSaveError(null)

    try {
      await createDeck(input)
      setIsCreating(false)
      decksQuery.reload()
    } catch (error) {
      setSaveError(getApiErrorMessage(error))
    } finally {
      setIsSaving(false)
    }
  }

  return (
    <AppLayout email={email}>
      <div className="flex flex-wrap items-center justify-between gap-4">
        <h1 className="text-3xl font-bold text-memorio-text">
          {t('dashboard.greeting', {
            greeting: t(greetingKeyForHour(new Date().getHours())),
            name: user.data ? displayNameFromEmail(email, t('dashboard.fallbackName')) : t('dashboard.fallbackName'),
          })}
        </h1>
        <Button onClick={openCreate}>{t('dashboard.newDeck')}</Button>
      </div>

      <div className="mt-7 space-y-8">
        <HeroBanner dueCount={0} streak={0} />
        <StatsRow />

        <section className="space-y-5">
          <h2 className="text-xl font-bold text-memorio-text">{t('dashboard.myDecks')}</h2>
          {decksQuery.isLoading ? (
            <PageLoader />
          ) : decksQuery.error ? (
            <p className="text-sm text-memorio-danger">{decksQuery.error}</p>
          ) : decks.length === 0 ? (
            <EmptyState
              icon="🗂"
              title={t('dashboard.noDecks.title')}
              description={t('dashboard.noDecks.description')}
              action={<Button onClick={openCreate}>{t('dashboard.newDeck')}</Button>}
            />
          ) : (
            <div className="space-y-6">
              <FeaturedDeck deck={featured} />
              {rest.length > 0 && (
                <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-4">
                  {rest.map((deck, index) => (
                    <DeckCard key={deck.id} deck={deck} accent={accentForIndex(index)} />
                  ))}
                </div>
              )}
            </div>
          )}
        </section>
      </div>

      {isCreating && (
        <DeckFormModal
          isSaving={isSaving}
          error={saveError}
          onSubmit={handleCreate}
          onClose={() => setIsCreating(false)}
        />
      )}
    </AppLayout>
  )
}
