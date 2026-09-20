import { useId, useState } from 'react'
import { SITE_NAME, SITE_TAGLINE, navLinks } from '../data/site'

type HeaderProps = {
  onNavigate: (page: 'home') => void
  onOpenPanel: () => void
}

export default function Header({ onNavigate, onOpenPanel }: HeaderProps) {
  const [query, setQuery] = useState('')
  const [menuOpen, setMenuOpen] = useState(false)
  const menuId = useId()

  function goHome() {
    setMenuOpen(false)
    onNavigate('home')
  }

  return (
    <header className="sticky top-0 z-30 border-b border-border bg-white/90 backdrop-blur">
      <div className="mx-auto max-w-6xl px-4 py-3 sm:px-6">
        {/* Single row on desktop: logo · menu · search */}
        <div className="flex items-center gap-4 lg:gap-6">
          <button
            onClick={goHome}
            className="flex shrink-0 items-center gap-3 text-left"
            aria-label={`${SITE_NAME} ana sayfa`}
          >
            <span
              aria-hidden
              className="flex h-10 w-10 items-center justify-center rounded-xl bg-gradient-to-br from-navy-600 to-teal-600 text-base font-bold text-white shadow-sm"
            >
              p.
            </span>
            <span className="leading-tight">
              <span className="block text-lg font-bold tracking-tight text-navy-900">
                {SITE_NAME}
              </span>
              <span className="hidden text-xs text-muted-foreground lg:block">{SITE_TAGLINE}</span>
            </span>
          </button>

          {/* Inline nav (desktop) */}
          <nav aria-label="Ana menü" className="hidden lg:block">
            <NavList onHome={goHome} onLinkClick={() => setMenuOpen(false)} />
          </nav>

          {/* Inline search (desktop) */}
          <form
            className="ml-auto hidden items-center lg:flex lg:w-64"
            role="search"
            onSubmit={(e) => e.preventDefault()}
          >
            <SearchField value={query} onChange={setQuery} id="site-search-desktop" />
          </form>

          {/* Yönetim paneli girişi (küçük) */}
          <button
            onClick={onOpenPanel}
            className="hidden shrink-0 items-center gap-1 rounded-full border border-border px-3 py-1.5 text-xs font-medium text-navy-500 transition-colors hover:border-teal-300 hover:bg-teal-50 hover:text-teal-700 lg:inline-flex"
            aria-label="Yönetim paneli"
          >
            <svg width="13" height="13" viewBox="0 0 24 24" fill="none" aria-hidden>
              <rect x="5" y="11" width="14" height="9" rx="2" stroke="currentColor" strokeWidth="2" />
              <path d="M8 11V8a4 4 0 0 1 8 0v3" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
            </svg>
            panel
          </button>

          {/* Mobile toggle */}
          <button
            type="button"
            onClick={() => setMenuOpen((v) => !v)}
            aria-expanded={menuOpen}
            aria-controls={menuId}
            aria-label={menuOpen ? 'Menüyü kapat' : 'Menüyü aç'}
            className="ml-auto rounded-md border border-border p-2 text-navy-700 transition-colors hover:bg-navy-50 lg:hidden"
          >
            <svg width="22" height="22" viewBox="0 0 24 24" fill="none" aria-hidden>
              {menuOpen ? (
                <path d="M6 6l12 12M18 6L6 18" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
              ) : (
                <path d="M4 7h16M4 12h16M4 17h16" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
              )}
            </svg>
          </button>
        </div>

        {/* Mobile collapsible menu + search */}
        <div id={menuId} className={`${menuOpen ? 'block' : 'hidden'} lg:hidden`}>
          <nav aria-label="Ana menü (mobil)">
            <NavList onHome={goHome} onLinkClick={() => setMenuOpen(false)} mobile />
          </nav>
          <form className="mt-3 flex items-center" role="search" onSubmit={(e) => e.preventDefault()}>
            <SearchField value={query} onChange={setQuery} id="site-search-mobile" />
          </form>
          <button
            onClick={() => {
              setMenuOpen(false)
              onOpenPanel()
            }}
            className="mt-3 flex w-full items-center justify-center gap-1.5 rounded-md border border-border px-3 py-2 text-sm font-medium text-navy-600 transition-colors hover:bg-teal-50 hover:text-teal-700"
          >
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" aria-hidden>
              <rect x="5" y="11" width="14" height="9" rx="2" stroke="currentColor" strokeWidth="2" />
              <path d="M8 11V8a4 4 0 0 1 8 0v3" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
            </svg>
            Yönetim Paneli
          </button>
        </div>
      </div>
    </header>
  )
}

type NavListProps = {
  onHome: () => void
  onLinkClick: () => void
  mobile?: boolean
}

function NavList({ onHome, onLinkClick, mobile = false }: NavListProps) {
  return (
    <ul
      className={
        mobile
          ? 'mt-3 flex flex-col gap-1 text-sm font-medium'
          : 'flex flex-wrap items-center gap-1 text-sm font-medium'
      }
    >
      {navLinks.map((link) =>
        link.key === 'home' ? (
          <li key={link.key}>
            <button
              onClick={onHome}
              className="block w-full rounded-md px-3 py-2 text-left text-navy-700 transition-colors hover:bg-teal-50 hover:text-teal-700"
            >
              {link.label}
            </button>
          </li>
        ) : (
          <li key={link.key}>
            <a
              href={`#${link.key}`}
              onClick={onLinkClick}
              className="block rounded-md px-3 py-2 text-navy-700 transition-colors hover:bg-teal-50 hover:text-teal-700"
            >
              {link.label}
            </a>
          </li>
        ),
      )}
    </ul>
  )
}

type SearchFieldProps = {
  id: string
  value: string
  onChange: (value: string) => void
}

function SearchField({ id, value, onChange }: SearchFieldProps) {
  return (
    <>
      <label htmlFor={id} className="sr-only">
        Site içi arama
      </label>
      <div className="flex w-full items-center rounded-full border border-border bg-navy-50 px-4 py-2 transition-colors focus-within:border-teal-400 focus-within:bg-white">
        <input
          id={id}
          type="search"
          value={value}
          onChange={(e) => onChange(e.target.value)}
          placeholder="Site içi arama..."
          className="w-full bg-transparent text-sm text-navy-900 outline-none placeholder:text-muted-foreground"
        />
        <button
          type="submit"
          aria-label="Ara"
          className="text-navy-500 transition-colors hover:text-teal-600"
        >
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" aria-hidden>
            <circle cx="11" cy="11" r="7" stroke="currentColor" strokeWidth="2" />
            <path d="m20 20-3.5-3.5" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
          </svg>
        </button>
      </div>
    </>
  )
}
