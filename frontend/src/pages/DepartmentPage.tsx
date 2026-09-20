import { useMemo, useState } from 'react'
import { AdBox, RecentPanel } from '../components/SidebarBlocks'
import {
  departments,
  levelTabs,
  recentItems,
  type Department,
  type ExamLevel,
  type ScoreCard,
} from '../data/site'

type DepartmentPageProps = {
  card: ScoreCard
  onOpenDepartment: (dept: Department) => void
  onNavigateHome: () => void
}

export default function DepartmentPage({
  card,
  onOpenDepartment,
  onNavigateHome,
}: DepartmentPageProps) {
  const [level, setLevel] = useState<ExamLevel>('lisans')
  const [query, setQuery] = useState('')

  const filtered = useMemo(() => {
    const q = query.trim().toLocaleLowerCase('tr-TR')
    return departments
      .filter((d) => d.level === level)
      .filter((d) => d.name.toLocaleLowerCase('tr-TR').includes(q))
  }, [level, query])

  return (
    <div className="mx-auto max-w-6xl px-4 py-6 sm:px-6">
      <nav aria-label="Konum" className="mb-4 text-sm text-muted-foreground">
        <button onClick={onNavigateHome} className="hover:text-navy-700">
          Ana Sayfa
        </button>
        <span aria-hidden> › </span>
        <span className="text-navy-800">{card.title}</span>
      </nav>

      <div className="grid gap-6 lg:grid-cols-[1fr_18rem]">
        <div className="rounded-lg border border-border bg-white">
          <div className="border-b border-border p-5">
            <h1 className="text-lg font-bold text-navy-900">{card.title} Taban Puanları</h1>

            <div className="mt-4 flex flex-wrap gap-2" role="group" aria-label="Öğrenim düzeyi">
              {levelTabs.map((tab) => (
                <button
                  key={tab.key}
                  type="button"
                  aria-pressed={level === tab.key}
                  onClick={() => setLevel(tab.key)}
                  className={`rounded-full px-4 py-1.5 text-sm font-medium transition-colors ${
                    level === tab.key
                      ? 'bg-navy-700 text-white'
                      : 'border border-border text-navy-700 hover:bg-navy-50'
                  }`}
                >
                  {tab.label}
                </button>
              ))}
            </div>

            <div className="mt-4">
              <label htmlFor="dept-search" className="sr-only">
                Bölüm ara
              </label>
              <input
                id="dept-search"
                type="search"
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                placeholder="Bölüm ara... (örn: hukuk, hemşirelik)"
                className="w-full rounded-md border border-border px-4 py-2.5 text-sm outline-none focus:border-navy-400"
              />
            </div>
          </div>

          <ul className="divide-y divide-border">
            {filtered.map((dept) => (
              <li key={dept.id}>
                <button
                  onClick={() => onOpenDepartment(dept)}
                  className="flex w-full items-center justify-between px-5 py-3.5 text-left transition-colors hover:bg-navy-50"
                >
                  <span className="text-sm font-medium text-navy-900">
                    {dept.name} Taban Puanları
                  </span>
                  <span aria-hidden className="text-navy-400">
                    ›
                  </span>
                </button>
              </li>
            ))}
            {filtered.length === 0 && (
              <li className="px-5 py-8 text-center text-sm text-muted-foreground">
                Aramanızla eşleşen bölüm bulunamadı.
              </li>
            )}
          </ul>
        </div>

        <aside className="space-y-6">
          <RecentPanel items={recentItems} />
          <AdBox />
        </aside>
      </div>
    </div>
  )
}
