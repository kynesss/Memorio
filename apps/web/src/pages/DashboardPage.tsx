import { tokenStore } from '../auth/tokenStore'

export function DashboardPage() {
  const isAuthenticated = Boolean(tokenStore.getAccessToken())

  return (
    <main className="flex min-h-screen items-center justify-center bg-memorio-bg px-6 text-center text-memorio-text">
      <div>
        <p className="text-sm font-semibold uppercase tracking-[0.22em] text-memorio-primary-light">Memorio</p>
        <h1 className="mt-3 text-3xl font-bold">Dashboard</h1>
        <p className="mt-3 text-sm text-memorio-muted">
          {isAuthenticated ? 'You are signed in.' : 'Dashboard view will be implemented in the next task.'}
        </p>
      </div>
    </main>
  )
}
