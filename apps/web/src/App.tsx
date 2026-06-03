import type { PropsWithChildren } from 'react'
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { AuthProvider, useAuth } from './auth/AuthContext'
import { ProtectedRoute } from './auth/ProtectedRoute'
import { PageLoader } from './components/common/Spinner'
import { DashboardPage } from './pages/DashboardPage'
import { DeckDetailPage } from './pages/DeckDetailPage'
import { LoginPage } from './pages/LoginPage'
import { RegisterPage } from './pages/RegisterPage'
import { StudySessionPage } from './pages/StudySessionPage'

function GuestRoute({ children }: PropsWithChildren) {
  const { status } = useAuth()

  if (status === 'checking') {
    return <PageLoader />
  }

  if (status === 'authenticated') {
    return <Navigate to="/dashboard" replace />
  }

  return <>{children}</>
}

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/login" element={<GuestRoute><LoginPage /></GuestRoute>} />
          <Route path="/register" element={<GuestRoute><RegisterPage /></GuestRoute>} />
          <Route
            path="/dashboard"
            element={(
              <ProtectedRoute>
                <DashboardPage />
              </ProtectedRoute>
            )}
          />
          <Route
            path="/decks/:deckId"
            element={(
              <ProtectedRoute>
                <DeckDetailPage />
              </ProtectedRoute>
            )}
          />
          <Route
            path="/decks/:deckId/study"
            element={(
              <ProtectedRoute>
                <StudySessionPage />
              </ProtectedRoute>
            )}
          />
          <Route path="*" element={<Navigate to="/login" replace />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  )
}

export default App
