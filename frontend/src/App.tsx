import { useState } from 'react'
import Header from './site/components/Header'
import Footer from './site/components/Footer'
import HomePage from './site/pages/HomePage'
import DepartmentPage from './site/pages/DepartmentPage'
import DetailPage from './site/pages/DetailPage'
import JobsPage from './site/pages/JobsPage'
import AdminApp from './admin/AdminApp'
import type { Department, ScoreCard } from './site/data/site'

type Route =
  | { page: 'home' }
  | { page: 'jobs' }
  | { page: 'departments'; card: ScoreCard }
  | { page: 'detail'; card: ScoreCard; department: Department }

export default function App() {
  const [route, setRoute] = useState<Route>({ page: 'home' })
  const [adminOpen, setAdminOpen] = useState(false)

  function goHome() {
    setRoute({ page: 'home' })
    window.scrollTo({ top: 0 })
  }

  function goJobs() {
    setRoute({ page: 'jobs' })
    window.scrollTo({ top: 0 })
  }

  if (adminOpen) return <AdminApp onExit={() => setAdminOpen(false)} />

  return (
    <div className="flex min-h-screen flex-col bg-gradient-to-b from-teal-50/40 via-navy-50/30 to-white">
      <Header onNavigate={goHome} onOpenJobs={goJobs} onOpenPanel={() => setAdminOpen(true)} />

      <main className="flex-1">
        {route.page === 'home' && (
          <HomePage
            onOpenScore={(card) => {
              setRoute({ page: 'departments', card })
              window.scrollTo({ top: 0 })
            }}
            onOpenJobs={goJobs}
          />
        )}

        {route.page === 'jobs' && <JobsPage onNavigateHome={goHome} />}

        {route.page === 'departments' && (
          <DepartmentPage
            card={route.card}
            onNavigateHome={goHome}
            onOpenDepartment={(department) => {
              setRoute({ page: 'detail', card: route.card, department })
              window.scrollTo({ top: 0 })
            }}
          />
        )}

        {route.page === 'detail' && (
          <DetailPage
            card={route.card}
            department={route.department}
            onNavigateHome={goHome}
            onBackToDepartments={() => {
              setRoute({ page: 'departments', card: route.card })
              window.scrollTo({ top: 0 })
            }}
          />
        )}
      </main>

      <Footer />
    </div>
  )
}
