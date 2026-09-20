import type { ScoreRow } from '../data/site'

function fmt(score: number) {
  return score.toLocaleString('tr-TR', { minimumFractionDigits: 5, maximumFractionDigits: 5 })
}

const columns = [
  { key: 'quota', label: 'Kont.' },
  { key: 'vacant', label: 'Boş' },
  { key: 'min', label: 'En Küçük' },
  { key: 'max', label: 'En Büyük' },
] as const

export default function ScoreTable({ rows }: { rows: ScoreRow[] }) {
  return (
    <div>
      {/* Desktop: table, all columns visible without horizontal scroll */}
      <table className="hidden w-full table-fixed border-collapse text-sm md:table">
        <thead>
          <tr className="border-b border-border bg-navy-50 text-left text-navy-800">
            <th scope="col" className="w-[34%] px-4 py-3 font-semibold">
              Kurum / Ünvan
            </th>
            <th scope="col" className="px-3 py-3 text-right font-semibold">
              Kont.
            </th>
            <th scope="col" className="px-3 py-3 text-right font-semibold">
              Boş
            </th>
            <th scope="col" className="px-3 py-3 text-right font-semibold">
              En Küçük
            </th>
            <th scope="col" className="px-3 py-3 text-right font-semibold">
              En Büyük
            </th>
            <th scope="col" className="px-3 py-3 font-semibold">
              Nitelik
            </th>
          </tr>
        </thead>
        <tbody>
          {rows.map((row) => (
            <tr key={row.id} className="border-b border-border align-top hover:bg-navy-50/60">
              <td className="px-4 py-3">
                <span className="block font-medium text-navy-900">
                  {row.institution} / {row.city}
                </span>
                <span className="block text-xs text-muted-foreground">{row.title}</span>
              </td>
              <td className="px-3 py-3 text-right tabular-nums">{row.quota}</td>
              <td className="px-3 py-3 text-right tabular-nums">{row.vacant}</td>
              <td className="px-3 py-3 text-right tabular-nums">{fmt(row.minScore)}</td>
              <td className="px-3 py-3 text-right tabular-nums">{fmt(row.maxScore)}</td>
              <td className="px-3 py-3 text-xs text-muted-foreground">{row.qualification}</td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* Mobile: card list */}
      <ul className="space-y-3 md:hidden">
        {rows.map((row) => (
          <li key={row.id} className="rounded-lg border border-border p-4">
            <p className="font-medium text-navy-900">
              {row.institution} / {row.city}
            </p>
            <p className="text-xs text-muted-foreground">{row.title}</p>
            <dl className="mt-3 grid grid-cols-2 gap-2 text-sm">
              {columns.map((col) => (
                <div key={col.key} className="flex justify-between gap-2 border-b border-border/70 pb-1">
                  <dt className="text-muted-foreground">{col.label}</dt>
                  <dd className="tabular-nums font-medium text-navy-900">
                    {col.key === 'quota' && row.quota}
                    {col.key === 'vacant' && row.vacant}
                    {col.key === 'min' && fmt(row.minScore)}
                    {col.key === 'max' && fmt(row.maxScore)}
                  </dd>
                </div>
              ))}
            </dl>
            <p className="mt-2 text-xs text-muted-foreground">Nitelik: {row.qualification}</p>
          </li>
        ))}
      </ul>
    </div>
  )
}
