import type { PropsWithChildren } from 'react'
import { Topbar } from './Topbar'

interface AppLayoutProps extends PropsWithChildren {
  email: string
}

export function AppLayout({ email, children }: AppLayoutProps) {
  return (
    <div className="min-h-screen bg-memorio-bg">
      <Topbar email={email} />
      <main className="mx-auto max-w-[1240px] px-5 py-10">{children}</main>
    </div>
  )
}
