import { useMemo, useState } from 'react'
import { AdBox } from '../components/SidebarBlocks'
import {
  jobCategories,
  jobListings,
  type JobCategory,
  type JobListing,
} from '../data/site'

type JobsPageProps = {
  onNavigateHome: () => void
}

const MONTHS_TR = [
  'Ocak',
  'Şubat',
  'Mart',
  'Nisan',
  'Mayıs',
  'Haziran',
  'Temmuz',
  'Ağustos',
  'Eylül',
  'Ekim',
  'Kasım',
  'Aralık',
]

/** Bir kategori düğümünün altındaki tüm yaprak anahtarlarını toplar. */
function leafKeys(node: JobCategory): string[] {
  if (!node.children?.length) return [node.key]
  return node.children.flatMap(leafKeys)
}

/** publishedAt (ISO) -> { day, month } görsel etiketi. */
function dayLabel(iso: string) {
  const d = new Date(iso)
  return { day: d.getDate(), month: MONTHS_TR[d.getMonth()], key: iso }
}

/** Kurum adından baş harf rozeti üretir. */
function initials(name: string) {
  return name
    .replace(/[^\p{L}\s]/gu, '')
    .split(/\s+/)
    .filter(Boolean)
    .slice(0, 2)
    .map((w) => w[0]?.toLocaleUpperCase('tr-TR'))
    .join('')
}

const BADGE_TONES = [
  'bg-teal-50 text-teal-700 ring-teal-600/20',
  'bg-navy-50 text-navy-700 ring-navy-600/20',
  'bg-sky-50 text-sky-700 ring-sky-600/20',
  'bg-amber-50 text-amber-700 ring-amber-600/20',
  'bg-cyan-50 text-cyan-700 ring-cyan-600/20',
]

export default function JobsPage({ onNavigateHome }: JobsPageProps) {
  const [selected, setSelected] = useState('all')
  const [query, setQuery] = useState('')
  const [openGroups, setOpenGroups] = useState<string[]>(['akademik'])

  // Seçili kategori altındaki yaprak anahtar kümesi
  const activeKeys = useMemo(() => {
    if (selected === 'all') return null
    const node = findNode(jobCategories, selected)
    return node ? new Set(leafKeys(node)) : new Set([selected])
  }, [selected])

  const filtered = useMemo(() => {
    const q = query.trim().toLocaleLowerCase('tr-TR')
    return jobListings
      .filter((j) => (activeKeys ? activeKeys.has(j.categoryKey) : true))
      .filter(
        (j) =>
          !q ||
          j.institution.toLocaleLowerCase('tr-TR').includes(q) ||
          j.summary.toLocaleLowerCase('tr-TR').includes(q),
      )
      .sort((a, b) => b.publishedAt.localeCompare(a.publishedAt))
  }, [activeKeys, query])

  // Timeline için tarihe göre grupla
  const groups = useMemo(() => {
    const map = new Map<string, JobListing[]>()
    for (const job of filtered) {
      const arr = map.get(job.publishedAt) ?? []
      arr.push(job)
      map.set(job.publishedAt, arr)
    }
    return [...map.entries()]
  }, [filtered])

  return (
    <div className="mx-auto max-w-6xl px-4 py-6 sm:px-6">
      <nav aria-label="Konum" className="mb-4 text-sm text-muted-foreground">
        <button onClick={onNavigateHome} className="hover:text-navy-700">
          Ana Sayfa
        </button>
        <span aria-hidden> › </span>
        <span className="text-navy-800">İlanlar</span>
      </nav>

      <div className="grid gap-6 lg:grid-cols-[19rem_1fr]">
        {/* Sol: arama + kategori ağacı */}
        <aside className="lg:sticky lg:top-20 lg:self-start">
          <div className="overflow-hidden rounded-2xl border border-border bg-white shadow-sm">
            <form
              className="border-b border-border p-3"
              role="search"
              onSubmit={(e) => e.preventDefault()}
            >
              <div className="flex items-center gap-2 rounded-xl border border-border bg-navy-50 px-3 py-2 transition-colors focus-within:border-teal-400 focus-within:bg-white">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" aria-hidden className="text-navy-400">
                  <circle cx="11" cy="11" r="7" stroke="currentColor" strokeWidth="2" />
                  <path d="m20 20-3.5-3.5" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
                </svg>
                <input
                  type="search"
                  value={query}
                  onChange={(e) => setQuery(e.target.value)}
                  placeholder="Kurum / Kuruluş Adı"
                  aria-label="İlan ara"
                  className="w-full bg-transparent text-sm text-navy-900 outline-none placeholder:text-muted-foreground"
                />
              </div>
            </form>

            <nav aria-label="İlan kategorileri" className="p-2">
              <ul className="space-y-0.5">
                {jobCategories.map((cat) => (
                  <CategoryNode
                    key={cat.key}
                    node={cat}
                    selected={selected}
                    onSelect={setSelected}
                    openGroups={openGroups}
                    onToggle={(key) =>
                      setOpenGroups((prev) =>
                        prev.includes(key) ? prev.filter((k) => k !== key) : [...prev, key],
                      )
                    }
                    count={countFor(cat)}
                  />
                ))}
              </ul>
            </nav>
          </div>

          <div className="mt-6 hidden lg:block">
            <AdBox className="h-64" />
          </div>
        </aside>

        {/* Sağ: başlık + timeline */}
        <div>
          <div className="mb-6 flex items-end justify-between border-b-2 border-teal-500/70 pb-3">
            <h1 className="flex items-center gap-2 text-xl font-bold uppercase tracking-wide text-navy-900">
              <svg width="20" height="20" viewBox="0 0 24 24" fill="none" aria-hidden className="text-teal-600">
                <path d="M4 11.5 12 5l8 6.5" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                <path d="M6 10v9h12v-9" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
              </svg>
              {selected === 'all' ? 'Tüm İlanlar' : findNode(jobCategories, selected)?.label}
            </h1>
            <span className="text-sm font-semibold text-teal-600">{filtered.length} ilan</span>
          </div>

          {groups.length === 0 ? (
            <div className="rounded-2xl border border-dashed border-border bg-white px-6 py-16 text-center text-sm text-muted-foreground">
              Bu kritere uygun ilan bulunamadı.
            </div>
          ) : (
            <div className="space-y-8">
              {groups.map(([iso, items]) => {
                const { day, month } = dayLabel(iso)
                return (
                  <div key={iso} className="grid grid-cols-[3.5rem_1fr] gap-3 sm:grid-cols-[5rem_1fr] sm:gap-5">
                    {/* Tarih sütunu */}
                    <div className="pt-1 text-right">
                      <div className="text-3xl font-extrabold leading-none text-navy-800 sm:text-4xl">
                        {day}
                      </div>
                      <div className="mt-1 text-xs font-medium uppercase tracking-wide text-teal-600">
                        {month}
                      </div>
                    </div>

                    {/* Kartlar + timeline çizgisi */}
                    <div className="relative space-y-3 border-l-2 border-navy-100 pl-4 sm:pl-6">
                      <span
                        aria-hidden
                        className="absolute -left-[7px] top-2 h-3 w-3 rounded-full border-2 border-teal-500 bg-white"
                      />
                      {items.map((job) => (
                        <JobCard key={job.id} job={job} />
                      ))}
                    </div>
                  </div>
                )
              })}
            </div>
          )}
        </div>
      </div>
    </div>
  )
}

function JobCard({ job }: { job: JobListing }) {
  const tone = BADGE_TONES[job.id % BADGE_TONES.length]
  return (
    <a
      href="#"
      className="group flex items-start gap-4 rounded-2xl border border-border bg-white p-4 shadow-sm transition-all hover:-translate-y-0.5 hover:border-teal-300 hover:shadow-md"
    >
      <span
        aria-hidden
        className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-gradient-to-br from-navy-600 to-teal-600 text-xs font-bold text-white shadow-sm"
      >
        {initials(job.institution)}
      </span>
      <div className="min-w-0 flex-1">
        <h3 className="text-sm font-bold uppercase leading-snug text-navy-900 group-hover:text-teal-700">
          {job.institution}
        </h3>
        <p className="mt-1 text-sm text-navy-600">
          {job.summary}{' '}
          <span className="font-semibold italic text-amber-600">
            ({job.startDate} - {job.endDate})
          </span>
        </p>
        <span
          className={`mt-2 inline-flex rounded-full px-2.5 py-0.5 text-[11px] font-semibold ring-1 ring-inset ${tone}`}
        >
          {job.categoryLabel}
        </span>
      </div>
    </a>
  )
}

type CategoryNodeProps = {
  node: JobCategory
  selected: string
  onSelect: (key: string) => void
  openGroups: string[]
  onToggle: (key: string) => void
  count: number
}

function CategoryNode({ node, selected, onSelect, openGroups, onToggle, count }: CategoryNodeProps) {
  const hasChildren = Boolean(node.children?.length)
  const isOpen = openGroups.includes(node.key)
  const isActive = selected === node.key

  return (
    <li>
      <div className="flex items-center">
        <button
          onClick={() => onSelect(node.key)}
          className={`flex flex-1 items-center justify-between gap-2 rounded-lg px-3 py-2 text-left text-sm font-semibold uppercase tracking-wide transition-colors ${
            isActive
              ? 'bg-teal-600 text-white'
              : 'text-navy-700 hover:bg-teal-50 hover:text-teal-700'
          }`}
        >
          <span className="truncate">{node.label}</span>
          <span
            className={`shrink-0 rounded-full px-1.5 py-0.5 text-[10px] font-bold ${
              isActive ? 'bg-white/20 text-white' : 'bg-navy-50 text-navy-500'
            }`}
          >
            {count}
          </span>
        </button>
        {hasChildren && (
          <button
            onClick={() => onToggle(node.key)}
            aria-label={isOpen ? 'Daralt' : 'Genişlet'}
            aria-expanded={isOpen}
            className="ml-1 rounded-md p-1.5 text-navy-400 transition-colors hover:bg-navy-50 hover:text-navy-700"
          >
            <svg
              width="16"
              height="16"
              viewBox="0 0 24 24"
              fill="none"
              aria-hidden
              className={`transition-transform ${isOpen ? 'rotate-180' : ''}`}
            >
              <path d="m6 9 6 6 6-6" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
            </svg>
          </button>
        )}
      </div>

      {hasChildren && isOpen && (
        <ul className="ml-3 mt-0.5 space-y-0.5 border-l border-navy-100 pl-2">
          {node.children!.map((child) => {
            const childActive = selected === child.key
            return (
              <li key={child.key}>
                <button
                  onClick={() => onSelect(child.key)}
                  className={`flex w-full items-center justify-between gap-2 rounded-lg px-3 py-1.5 text-left text-[13px] transition-colors ${
                    childActive
                      ? 'bg-teal-50 font-semibold text-teal-700'
                      : 'text-navy-600 hover:bg-navy-50 hover:text-navy-900'
                  }`}
                >
                  <span className="truncate">{child.label}</span>
                  <span className="shrink-0 text-[10px] font-semibold text-navy-400">
                    {countFor(child)}
                  </span>
                </button>
              </li>
            )
          })}
        </ul>
      )}
    </li>
  )
}

// --- yardımcılar ---

function findNode(nodes: JobCategory[], key: string): JobCategory | undefined {
  for (const n of nodes) {
    if (n.key === key) return n
    if (n.children) {
      const found = findNode(n.children, key)
      if (found) return found
    }
  }
  return undefined
}

function countFor(node: JobCategory): number {
  if (node.key === 'all') return jobListings.length
  const keys = new Set(leafKeys(node))
  return jobListings.filter((j) => keys.has(j.categoryKey)).length
}
