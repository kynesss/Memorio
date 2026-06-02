import { Link, useLocation } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { initialsFromEmail } from '../../utils/user'

interface TopbarProps {
  email: string
}

export function Topbar({ email }: TopbarProps) {
  const { t } = useTranslation()
  const { pathname } = useLocation()
  const isDashboard = pathname === '/dashboard'
  const isDecks = pathname.startsWith('/decks')

  return (
    <header className="sticky top-0 z-30 border-b border-memorio-border bg-memorio-bg/90 backdrop-blur">
      <div className="mx-auto flex h-16 max-w-[1240px] items-center gap-8 px-5">
        <Link to="/dashboard" className="flex items-center gap-2.5">
          <span className="flex size-8 items-center justify-center rounded-lg bg-memorio-primary text-sm font-bold text-white">M</span>
          <span className="text-lg font-bold text-memorio-text">Memorio</span>
        </Link>
        <nav className="flex items-center gap-1">
          <NavLink to="/dashboard" label={t('navigation.dashboard')} active={isDashboard} />
          <NavLink to="/dashboard" label={t('navigation.myDecks')} active={isDecks} />
          <NavItemDisabled label={t('navigation.study')} />
          <NavItemDisabled label={t('navigation.settings')} />
        </nav>
        <span
          className="ml-auto flex size-9 items-center justify-center rounded-full bg-memorio-primary text-xs font-bold text-white"
          title={email}
        >
          {email ? initialsFromEmail(email) : '·'}
        </span>
      </div>
    </header>
  )
}

function NavLink({ to, label, active }: { to: string; label: string; active: boolean }) {
  return (
    <Link
      to={to}
      className={`rounded-lg px-3 py-2 text-sm font-medium transition ${
        active ? 'text-memorio-primary-light' : 'text-memorio-muted hover:text-memorio-text'
      }`}
    >
      {label}
    </Link>
  )
}

function NavItemDisabled({ label }: { label: string }) {
  const { t } = useTranslation()

  return (
    <span className="cursor-not-allowed rounded-lg px-3 py-2 text-sm font-medium text-memorio-subtle" title={t('common.comingSoon')}>
      {label}
    </span>
  )
}
