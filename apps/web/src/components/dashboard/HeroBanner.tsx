import { Button } from '../common/Button'

interface HeroBannerProps {
  dueCount: number
  streak: number
}

export function HeroBanner({ dueCount, streak }: HeroBannerProps) {
  const hasDue = dueCount > 0

  return (
    <section className="relative overflow-hidden rounded-2xl border border-memorio-border bg-[#17122b] px-8 py-9 before:absolute before:inset-x-0 before:top-0 before:h-0.75 before:bg-memorio-primary before:content-['']">
      <div className="pointer-events-none absolute -right-12 top-1/2 size-72 -translate-y-1/2 rounded-full bg-memorio-primary/15 blur-2xl" />
      <div className="relative max-w-xl">
        <h2 className="text-3xl font-bold text-memorio-text">
          {hasDue ? `You have ${dueCount} cards due today` : "You're all caught up"}
        </h2>
        <p className="mt-2 text-sm text-memorio-muted">
          {streak > 0
            ? `Keep your streak going — ${streak} days and counting 🔥`
            : 'Create a deck and start building your memory.'}
        </p>
        <Button className="mt-6" disabled={!hasDue} title={hasDue ? undefined : 'Available once you have cards due'}>
          Study now →
        </Button>
      </div>
    </section>
  )
}
