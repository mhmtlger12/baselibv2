import { useMemo, useState } from 'react'
import ScoreTable from '../components/ScoreTable'
import CommentThread from '../components/CommentThread'
import { AdBox, RecentPanel } from '../components/SidebarBlocks'
import {
  comments,
  detailPeriods,
  recentItems,
  scoreRows,
  type Department,
  type ScoreCard,
} from '../data/site'

const PAGE_SIZE = 5

type DetailPageProps = {
  card: ScoreCard
  department: Department
  onNavigateHome: () => void
  onBackToDepartments: () => void
}

export default function DetailPage({
  card,
  department,
  onNavigateHome,
  onBackToDepartments,
}: DetailPageProps) {
  const [period, setPeriod] = useState(detailPeriods[0])
  const [institution, setInstitution] = useState('')
  const [city, setCity] = useState('')
  const [applied, setApplied] = useState({ institution: '', city: '' })
  const [page, setPage] = useState(1)

  const filtered = useMemo(() => {
    const inst = applied.institution.toLocaleLowerCase('tr-TR')
    const cty = applied.city.toLocaleLowerCase('tr-TR')
    return scoreRows.filter(
      (r) =>
        r.institution.toLocaleLowerCase('tr-TR').includes(inst) &&
        r.city.toLocaleLowerCase('tr-TR').includes(cty),
    )
  }, [applied])

  const pageCount = Math.max(1, Math.ceil(filtered.length / PAGE_SIZE))
  const currentPage = Math.min(page, pageCount)
  const pageRows = filtered.slice((currentPage - 1) * PAGE_SIZE, currentPage * PAGE_SIZE)

  const summary = useMemo(() => {
    if (filtered.length === 0) return null
    return {
      totalQuota: filtered.reduce((s, r) => s + r.quota, 0),
      minScore: Math.min(...filtered.map((r) => r.minScore)),
      maxScore: Math.max(...filtered.map((r) => r.maxScore)),
    }
  }, [filtered])

  function handleSearch(e: React.FormEvent) {
    e.preventDefault()
    setApplied({ institution, city })
    setPage(1)
  }

  function handleClear() {
    setInstitution('')
    setCity('')
    setApplied({ institution: '', city: '' })
    setPage(1)
  }

  return (
    <div className="mx-auto max-w-6xl px-4 py-6 sm:px-6">
      <nav aria-label="Konum" className="mb-4 text-sm text-muted-foreground">
        <button onClick={onNavigateHome} className="hover:text-navy-700">
          Ana Sayfa
        </button>
        <span aria-hidden> › </span>
        <button onClick={onBackToDepartments} className="hover:text-navy-700">
          {card.title}
        </button>
        <span aria-hidden> › </span>
        <span className="text-navy-800">{department.name}</span>
      </nav>

      <div className="grid gap-6 lg:grid-cols-[1fr_18rem] lg:items-start">
        <div className="space-y-5">
          <div className="rounded-lg border border-border bg-white p-5">
            <div className="flex flex-wrap items-center justify-between gap-3">
              <h1 className="text-lg font-bold text-navy-900">
                {department.name} Taban Puanları
              </h1>
              <div className="flex items-center gap-2">
                <label htmlFor="period" className="text-sm text-muted-foreground">
                  Dönem:
                </label>
                <select
                  id="period"
                  value={period}
                  onChange={(e) => setPeriod(e.target.value)}
                  className="rounded-md border border-border px-3 py-1.5 text-sm outline-none focus:border-navy-400"
                >
                  {detailPeriods.map((p) => (
                    <option key={p} value={p}>
                      {p}
                    </option>
                  ))}
                </select>
              </div>
            </div>

            {/* Search */}
            <form onSubmit={handleSearch} className="mt-4 flex flex-col gap-2 sm:flex-row">
              <label htmlFor="inst-search" className="sr-only">
                Kurum ara
              </label>
              <input
                id="inst-search"
                value={institution}
                onChange={(e) => setInstitution(e.target.value)}
                placeholder="Kurum ara..."
                className="flex-1 rounded-md border border-border px-3 py-2 text-sm outline-none focus:border-navy-400"
              />
              <label htmlFor="city-search" className="sr-only">
                Şehir ara
              </label>
              <input
                id="city-search"
                value={city}
                onChange={(e) => setCity(e.target.value)}
                placeholder="Şehir ara..."
                className="flex-1 rounded-md border border-border px-3 py-2 text-sm outline-none focus:border-navy-400"
              />
              <div className="flex gap-2">
                <button
                  type="submit"
                  className="rounded-md bg-navy-700 px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-navy-800"
                >
                  Ara
                </button>
                <button
                  type="button"
                  onClick={handleClear}
                  className="rounded-md border border-border px-4 py-2 text-sm font-medium text-navy-700 transition-colors hover:bg-navy-50"
                >
                  Temizle
                </button>
              </div>
            </form>
          </div>

          {/* Summary cards */}
          {summary && (
            <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
              <SummaryCard label="Toplam Kadro" value={summary.totalQuota.toString()} />
              <SummaryCard label="En Düşük Puan" value={fmt(summary.minScore)} />
              <SummaryCard label="En Yüksek Puan" value={fmt(summary.maxScore)} />
            </div>
          )}

          {/* Table */}
          <div className="rounded-lg border border-border bg-white p-2 sm:p-4">
            {pageRows.length > 0 ? (
              <ScoreTable rows={pageRows} />
            ) : (
              <p className="px-4 py-10 text-center text-sm text-muted-foreground">
                Aramanızla eşleşen kayıt bulunamadı.
              </p>
            )}

            {/* Pagination */}
            {filtered.length > 0 && (
              <div className="mt-4 flex flex-wrap items-center justify-between gap-3 px-2 pb-2">
                <p className="text-xs text-muted-foreground" aria-live="polite">
                  {filtered.length} kayıttan {(currentPage - 1) * PAGE_SIZE + 1}–
                  {Math.min(currentPage * PAGE_SIZE, filtered.length)} arası gösteriliyor
                </p>
                <nav aria-label="Sayfalama" className="flex items-center gap-1">
                  {Array.from({ length: pageCount }, (_, i) => i + 1).map((p) => (
                    <button
                      key={p}
                      onClick={() => setPage(p)}
                      aria-label={`Sayfa ${p}`}
                      aria-current={p === currentPage ? 'page' : undefined}
                      className={`h-8 min-w-8 rounded-md px-2 text-sm font-medium transition-colors ${
                        p === currentPage
                          ? 'bg-navy-700 text-white'
                          : 'border border-border text-navy-700 hover:bg-navy-50'
                      }`}
                    >
                      {p}
                    </button>
                  ))}
                </nav>
              </div>
            )}
          </div>

          <CommentThread initial={comments} />
        </div>

        {/* Sticky sidebar */}
        <aside className="space-y-6 lg:sticky lg:top-24">
          <AdBox />
          <RecentPanel items={recentItems} />
        </aside>
      </div>
    </div>
  )
}

function fmt(score: number) {
  return score.toLocaleString('tr-TR', { minimumFractionDigits: 5, maximumFractionDigits: 5 })
}

function SummaryCard({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-lg border border-border bg-white p-4">
      <p className="text-sm text-muted-foreground">{label}</p>
      <p className="mt-1 text-2xl font-bold tabular-nums text-navy-900">{value}</p>
    </div>
  )
}
