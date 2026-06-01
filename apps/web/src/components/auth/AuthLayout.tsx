import type { PropsWithChildren } from 'react'

export function AuthLayout({ children }: PropsWithChildren) {
  return (
    <main className="relative flex min-h-screen items-center justify-center overflow-hidden bg-memorio-bg px-5 py-10">
      <div className="pointer-events-none absolute -left-25 -top-25 size-150 rounded-full bg-[#151227]" />
      <div className="pointer-events-none absolute -bottom-25 -right-15 size-100 rounded-full bg-[#0c171f]" />
      {children}
    </main>
  )
}
