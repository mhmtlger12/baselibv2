import type { Announcement, RecentItem } from '../data/site'

function Panel({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <section className="overflow-hidden rounded-xl border border-border bg-white shadow-sm">
      <h3 className="flex items-center gap-2 border-b border-border bg-navy-50/60 px-4 py-3 text-sm font-semibold text-navy-800">
        <span aria-hidden className="h-4 w-1 rounded-full bg-teal-500" />
        {title}
      </h3>
      <div className="p-4">{children}</div>
    </section>
  )
}

export function RecentPanel({ items }: { items: RecentItem[] }) {
  return (
    <Panel title="Son Eklenenler">
      <ul className="space-y-3">
        {items.map((item) => (
          <li key={item.id}>
            <a href="#" className="group block border-l-2 border-transparent pl-3 transition-colors hover:border-teal-500">
              <p className="text-sm font-medium text-navy-700 group-hover:text-teal-700">
                {item.title}
              </p>
              <p className="mt-0.5 text-xs text-muted-foreground">{item.date}</p>
            </a>
          </li>
        ))}
      </ul>
    </Panel>
  )
}

export function AnnouncementsPanel({ items }: { items: Announcement[] }) {
  return (
    <Panel title="ÖSYM Duyuruları">
      <ul className="space-y-3">
        {items.map((item) => (
          <li key={item.id} className="flex gap-2">
            <span aria-hidden className="mt-1 h-1.5 w-1.5 shrink-0 rounded-full bg-teal-500" />
            <div>
              <a href="#" className="text-sm font-medium text-navy-700 hover:text-navy-900">
                {item.title}
              </a>
              <p className="mt-0.5 text-xs text-muted-foreground">{item.date}</p>
            </div>
          </li>
        ))}
      </ul>
    </Panel>
  )
}

export function AdBox({
  label = 'Reklam Alanı',
  className = 'h-40',
}: {
  label?: string
  className?: string
}) {
  return (
    <div
      className={`relative flex flex-col items-center justify-center overflow-hidden rounded-xl border border-dashed border-navy-300 bg-navy-50/70 px-4 text-center ${className}`}
      role="complementary"
      aria-label={label}
    >
      <span aria-hidden className="absolute inset-x-0 top-0 h-1 bg-gradient-to-r from-teal-400 via-navy-300 to-teal-400" />
      <span className="rounded-full border border-navy-200 bg-white px-2.5 py-1 text-[10px] font-bold uppercase tracking-[0.16em] text-navy-500">
        Sponsorlu içerik
      </span>
      <span className="mt-2 text-sm font-semibold text-navy-700">{label}</span>
      <span className="mt-1 text-xs text-muted-foreground">Reklam</span>
    </div>
  )
}
