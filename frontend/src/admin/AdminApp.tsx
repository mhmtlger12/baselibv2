import { useState, type ReactNode } from 'react'
import {
  IconDashboard,
  IconUsers,
  IconRoles,
  IconKey,
  IconTree,
  IconMenu,
  IconGear,
  IconActivity,
  IconTrash,
  IconLogout,
  IconChevron,
  IconExternal,
  IconLock,
  IconLayers,
  IconImage,
  IconAd,
  IconClock,
  IconStar,
  IconChat,
  IconMail,
} from './icons'
import Dashboard from './sections/Dashboard'
import Users from './sections/Users'
import Roles from './sections/Roles'
import Permissions from './sections/Permissions'
import Departments from './sections/Departments'
import Menus from './sections/Menus'
import Settings from './sections/Settings'
import AuditLogs from './sections/AuditLogs'
import RecycleBin from './sections/RecycleBin'
import Profile from './sections/Profile'
import ContentManager from './content/ContentManager'
import { contentConfigs } from './content/contentConfig'
import { currentUser } from './adminData'

/** Menü ağacı: yaprak düğümler bir bölüm açar, üst düğümler açılır/kapanır. */
type NavNode = {
  key: string
  label: string
  icon?: ReactNode
  children?: NavNode[]
}

const nav: NavNode[] = [
  { key: 'dashboard', label: 'Dashboard', icon: <IconDashboard className="h-5 w-5" /> },
  {
    key: 'content',
    label: 'İçerik Yönetimi',
    icon: <IconLayers className="h-5 w-5" />,
    children: [
      { key: 'content/slider', label: 'Slider', icon: <IconImage className="h-4 w-4" /> },
      { key: 'content/ads', label: 'Reklam Alanları', icon: <IconAd className="h-4 w-4" /> },
      { key: 'content/countdowns', label: 'Sınav Sayaçları', icon: <IconClock className="h-4 w-4" /> },
      {
        key: 'sidebars',
        label: 'Sidebarlar',
        icon: <IconMenu className="h-4 w-4" />,
        children: [
          { key: 'content/sidebar-recent', label: 'Son Eklenenler' },
          { key: 'content/sidebar-osym', label: 'ÖSYM Duyuruları' },
        ],
      },
      {
        key: 'scores',
        label: 'Taban Puanları',
        icon: <IconTree className="h-4 w-4" />,
        children: [
          { key: 'content/score-cards', label: 'Kategori Kartları' },
          { key: 'content/departments', label: 'Bölümler' },
          { key: 'content/taban-kpss', label: 'KPSS' },
          { key: 'content/taban-dgs', label: 'DGS' },
          { key: 'content/taban-yks', label: 'YKS' },
        ],
      },
      { key: 'content/popular', label: 'Popüler Üniversiteler', icon: <IconStar className="h-4 w-4" /> },
      { key: 'content/comments', label: 'Yorumlar', icon: <IconChat className="h-4 w-4" /> },
      { key: 'content/messages', label: 'İletişim Mesajları', icon: <IconMail className="h-4 w-4" /> },
    ],
  },
  { key: 'users', label: 'Kullanıcılar', icon: <IconUsers className="h-5 w-5" /> },
  { key: 'roles', label: 'Roller', icon: <IconRoles className="h-5 w-5" /> },
  { key: 'permissions', label: 'İzinler', icon: <IconKey className="h-5 w-5" /> },
  { key: 'departments', label: 'Departmanlar', icon: <IconTree className="h-5 w-5" /> },
  { key: 'menus', label: 'Menüler', icon: <IconMenu className="h-5 w-5" /> },
  { key: 'settings', label: 'Sistem Ayarları', icon: <IconGear className="h-5 w-5" /> },
  { key: 'audit', label: 'Sistem Hareketleri', icon: <IconActivity className="h-5 w-5" /> },
  { key: 'recycle', label: 'Çöp Kutusu', icon: <IconTrash className="h-5 w-5" /> },
]

const coreTitles: Record<string, string> = {
  dashboard: 'Dashboard',
  users: 'Kullanıcılar',
  roles: 'Roller',
  permissions: 'İzinler',
  departments: 'Departmanlar',
  menus: 'Menüler',
  settings: 'Sistem Ayarları',
  audit: 'Sistem Hareketleri',
  recycle: 'Çöp Kutusu',
  profile: 'Profilim',
}

function getTitle(section: string): string {
  if (section.startsWith('content/')) return contentConfigs[section]?.title ?? 'İçerik Yönetimi'
  return coreTitles[section] ?? 'Yönetim Paneli'
}

/** Bir düğümün alt ağacında verilen bölümün olup olmadığını kontrol eder. */
function subtreeContains(node: NavNode, section: string): boolean {
  if (node.key === section) return true
  return node.children?.some((c) => subtreeContains(c, section)) ?? false
}

export default function AdminApp({ onExit }: { onExit: () => void }) {
  const [authed, setAuthed] = useState(false)

  if (!authed) return <Login onSuccess={() => setAuthed(true)} onExit={onExit} />

  return <Shell onExit={onExit} onLogout={() => setAuthed(false)} />
}

function Shell({ onExit, onLogout }: { onExit: () => void; onLogout: () => void }) {
  const [section, setSection] = useState<string>('dashboard')
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const [userMenuOpen, setUserMenuOpen] = useState(false)
  const [activeRole, setActiveRole] = useState(currentUser.roles[0])

  function renderContent() {
    if (section.startsWith('content/')) {
      const config = contentConfigs[section]
      return config ? <ContentManager key={section} config={config} /> : null
    }
    switch (section) {
      case 'dashboard':
        return <Dashboard />
      case 'users':
        return <Users />
      case 'roles':
        return <Roles />
      case 'permissions':
        return <Permissions />
      case 'departments':
        return <Departments />
      case 'menus':
        return <Menus />
      case 'settings':
        return <Settings />
      case 'audit':
        return <AuditLogs />
      case 'recycle':
        return <RecycleBin />
      case 'profile':
        return <Profile activeRole={activeRole} onSwitchRole={setActiveRole} />
      default:
        return <Dashboard />
    }
  }

  function select(key: string) {
    setSection(key)
    setSidebarOpen(false)
  }

  return (
    <div className="min-h-screen bg-navy-50/50 lg:pl-72">
      {/* Sidebar */}
      <aside
        className={`fixed inset-y-0 left-0 z-40 flex w-72 flex-col bg-navy-900 text-navy-100 transition-transform lg:translate-x-0 ${
          sidebarOpen ? 'translate-x-0' : '-translate-x-full'
        }`}
      >
        <div className="flex items-center gap-3 px-6 py-5">
          <span className="flex h-10 w-10 items-center justify-center rounded-xl bg-gradient-to-br from-teal-400 to-teal-600 text-base font-bold text-white shadow-lg shadow-teal-900/40">
            p.
          </span>
          <div className="leading-tight">
            <span className="block text-base font-bold text-white">puannokta</span>
            <span className="block text-[11px] uppercase tracking-widest text-teal-300/80">
              Yönetim Paneli
            </span>
          </div>
        </div>

        <nav className="flex-1 space-y-1 overflow-y-auto px-3 py-2">
          {nav.map((node) => (
            <NavItem key={node.key} node={node} depth={0} section={section} onSelect={select} />
          ))}
        </nav>

        <div className="space-y-1 border-t border-white/10 px-3 py-3">
          <button
            onClick={onExit}
            className="flex w-full items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium text-navy-200 transition-colors hover:bg-white/5 hover:text-white"
          >
            <IconExternal className="h-5 w-5 text-navy-300" />
            Siteye Dön
          </button>
          <button
            onClick={onLogout}
            className="flex w-full items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium text-rose-200 transition-colors hover:bg-rose-500/10 hover:text-rose-100"
          >
            <IconLogout className="h-5 w-5" />
            Çıkış
          </button>
        </div>
      </aside>

      {/* Mobile overlay */}
      {sidebarOpen && (
        <button
          aria-label="Menüyü kapat"
          onClick={() => setSidebarOpen(false)}
          className="fixed inset-0 z-30 bg-navy-900/50 backdrop-blur-sm lg:hidden"
        />
      )}

      {/* Topbar */}
      <header className="sticky top-0 z-20 border-b border-navy-100 bg-white/80 backdrop-blur">
        <div className="flex items-center gap-4 px-4 py-4 sm:px-8">
          <button
            onClick={() => setSidebarOpen(true)}
            className="rounded-lg border border-navy-100 p-2 text-navy-600 transition-colors hover:bg-navy-50 lg:hidden"
            aria-label="Menüyü aç"
          >
            <IconMenu className="h-5 w-5" />
          </button>
          <div>
            <p className="text-[11px] font-semibold uppercase tracking-widest text-teal-600">
              Yönetim Paneli
            </p>
            <h1 className="text-xl font-bold tracking-tight text-navy-900">{getTitle(section)}</h1>
          </div>

          <div className="relative ml-auto">
            <button
              onClick={() => setUserMenuOpen((v) => !v)}
              className="flex items-center gap-2 rounded-full border border-navy-100 bg-white py-1.5 pl-1.5 pr-3 transition-colors hover:bg-navy-50"
            >
              <span className="flex h-8 w-8 items-center justify-center rounded-full bg-navy-800 text-xs font-bold text-white">
                {currentUser.firstName[0]}
              </span>
              <span className="hidden leading-tight text-left sm:block">
                <span className="block text-sm font-semibold text-navy-900">
                  {currentUser.firstName} {currentUser.lastName}
                </span>
                <span className="block text-xs text-muted-foreground">{activeRole}</span>
              </span>
              <IconChevron
                className={`hidden h-4 w-4 text-navy-400 transition-transform sm:block ${userMenuOpen ? 'rotate-180' : ''}`}
              />
            </button>

            {userMenuOpen && (
              <>
                <button
                  aria-label="Kapat"
                  onClick={() => setUserMenuOpen(false)}
                  className="fixed inset-0 z-10 cursor-default"
                />
                <div className="absolute right-0 z-20 mt-2 w-52 overflow-hidden rounded-xl border border-navy-100 bg-white py-1.5 shadow-xl">
                  <button
                    onClick={() => {
                      setSection('profile')
                      setUserMenuOpen(false)
                    }}
                    className="flex w-full items-center gap-2.5 px-4 py-2.5 text-left text-sm font-medium text-navy-700 transition-colors hover:bg-navy-50"
                  >
                    <IconUsers className="h-4 w-4 text-navy-400" />
                    Profilim
                  </button>
                  <button
                    onClick={onExit}
                    className="flex w-full items-center gap-2.5 px-4 py-2.5 text-left text-sm font-medium text-navy-700 transition-colors hover:bg-navy-50"
                  >
                    <IconExternal className="h-4 w-4 text-navy-400" />
                    Siteye Dön
                  </button>
                  <div className="my-1 border-t border-navy-50" />
                  <button
                    onClick={onLogout}
                    className="flex w-full items-center gap-2.5 px-4 py-2.5 text-left text-sm font-medium text-rose-600 transition-colors hover:bg-rose-50"
                  >
                    <IconLogout className="h-4 w-4" />
                    Çıkış
                  </button>
                </div>
              </>
            )}
          </div>
        </div>
      </header>

      <main className="px-4 py-6 sm:px-8 sm:py-8">
        <div className="mx-auto max-w-7xl">{renderContent()}</div>
      </main>
    </div>
  )
}

/** Sidebar menü düğümü — grup ise açılır/kapanır, yaprak ise bölüm seçer. */
function NavItem({
  node,
  depth,
  section,
  onSelect,
}: {
  node: NavNode
  depth: number
  section: string
  onSelect: (key: string) => void
}) {
  const isGroup = !!node.children?.length
  const [open, setOpen] = useState(() => (isGroup ? subtreeContains(node, section) : false))
  const active = section === node.key
  const hasActiveChild = isGroup && subtreeContains(node, section)

  return (
    <div>
      <button
        onClick={() => (isGroup ? setOpen((v) => !v) : onSelect(node.key))}
        style={{ paddingLeft: depth * 16 + 12 }}
        className={`group relative flex w-full items-center gap-3 rounded-xl py-2.5 pr-3 text-sm font-medium transition-colors ${
          active
            ? 'bg-white/10 text-white'
            : hasActiveChild
              ? 'text-white'
              : 'text-navy-200 hover:bg-white/5 hover:text-white'
        }`}
      >
        {active && (
          <span className="absolute left-0 top-1/2 h-6 w-1 -translate-y-1/2 rounded-r-full bg-teal-400" />
        )}
        {node.icon ? (
          <span className={active || hasActiveChild ? 'text-teal-300' : 'text-navy-300 group-hover:text-teal-300'}>
            {node.icon}
          </span>
        ) : (
          <span
            className={`ml-1 h-1.5 w-1.5 rounded-full ${active ? 'bg-teal-300' : 'bg-navy-400 group-hover:bg-teal-300'}`}
          />
        )}
        <span className="flex-1 text-left">{node.label}</span>
        {isGroup && (
          <IconChevron className={`h-4 w-4 text-navy-400 transition-transform ${open ? '' : '-rotate-90'}`} />
        )}
      </button>

      {isGroup && open && (
        <div className="mt-1 space-y-1">
          {node.children!.map((child) => (
            <NavItem key={child.key} node={child} depth={depth + 1} section={section} onSelect={onSelect} />
          ))}
        </div>
      )}
    </div>
  )
}

function Login({ onSuccess, onExit }: { onSuccess: () => void; onExit: () => void }) {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')

  function submit(e: React.FormEvent) {
    e.preventDefault()
    if (username === 'admin' && password === 'admin') {
      setError('')
      onSuccess()
    } else {
      setError('Kullanıcı adı veya şifre hatalı.')
    }
  }

  return (
    <div className="relative flex min-h-screen items-center justify-center overflow-hidden bg-navy-900 px-4">
      {/* Arka plan dekoru */}
      <div className="pointer-events-none absolute inset-0">
        <div className="absolute -left-32 -top-32 h-96 w-96 rounded-full bg-teal-500/20 blur-3xl" />
        <div className="absolute -bottom-32 -right-32 h-96 w-96 rounded-full bg-navy-500/30 blur-3xl" />
      </div>

      <div className="relative w-full max-w-sm">
        <div className="mb-8 flex flex-col items-center text-center">
          <span className="flex h-14 w-14 items-center justify-center rounded-2xl bg-gradient-to-br from-teal-400 to-teal-600 text-xl font-bold text-white shadow-lg shadow-teal-900/40">
            p.
          </span>
          <h1 className="mt-4 text-2xl font-bold text-white">puannokta</h1>
          <p className="text-sm text-navy-300">Yönetim Paneli Girişi</p>
        </div>

        <form
          onSubmit={submit}
          className="space-y-4 rounded-2xl border border-white/10 bg-white/5 p-6 backdrop-blur"
        >
          <label className="block">
            <span className="mb-1.5 block text-sm font-medium text-navy-100">Kullanıcı Adı</span>
            <input
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              autoComplete="username"
              placeholder="admin"
              className="w-full rounded-xl border border-white/10 bg-navy-800/60 px-3.5 py-2.5 text-sm text-white outline-none transition-colors placeholder:text-navy-400 focus:border-teal-400 focus:ring-2 focus:ring-teal-500/30"
            />
          </label>
          <label className="block">
            <span className="mb-1.5 block text-sm font-medium text-navy-100">Şifre</span>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              autoComplete="current-password"
              placeholder="••••••••"
              className="w-full rounded-xl border border-white/10 bg-navy-800/60 px-3.5 py-2.5 text-sm text-white outline-none transition-colors placeholder:text-navy-400 focus:border-teal-400 focus:ring-2 focus:ring-teal-500/30"
            />
          </label>

          {error && (
            <p className="rounded-lg bg-rose-500/15 px-3 py-2 text-sm text-rose-200">{error}</p>
          )}

          <button
            type="submit"
            className="flex w-full items-center justify-center gap-2 rounded-xl bg-teal-500 px-4 py-2.5 text-sm font-semibold text-white transition-colors hover:bg-teal-400"
          >
            <IconLock className="h-4 w-4" />
            Giriş Yap
          </button>

          <p className="text-center text-xs text-navy-400">Demo giriş: admin / admin</p>
        </form>

        <button
          onClick={onExit}
          className="mx-auto mt-6 block text-sm text-navy-300 transition-colors hover:text-white"
        >
          ← Siteye dön
        </button>
      </div>
    </div>
  )
}
