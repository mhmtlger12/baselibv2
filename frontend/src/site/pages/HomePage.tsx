import Slider from '../components/Slider'
import CountdownCard from '../components/CountdownCard'
import SectionHeading from '../components/SectionHeading'
import { AdBox, AnnouncementsPanel, RecentPanel } from '../components/SidebarBlocks'
import {
  announcements,
  countdowns,
  jobListings,
  recentItems,
  scoreCards,
  slides,
  type ScoreCard,
} from '../data/site'

type HomePageProps = {
  onOpenScore: (card: ScoreCard) => void
  onOpenJobs: () => void
}

export default function HomePage({ onOpenScore, onOpenJobs }: HomePageProps) {
  return (
    <div className="mx-auto max-w-6xl px-4 py-6 sm:px-6">
      {/* Top band: slider and advertising keep the original desktop proportions */}
      <div className="grid gap-6 lg:grid-cols-3">
        <div className="lg:col-span-2">
          <Slider slides={slides} />
        </div>
        <AdBox className="h-28 lg:h-72" />
      </div>

      {/* Countdown row: all exams together, same style */}
      <section aria-label="Kalan zaman sayaçları" className="mt-6">
        <div className="grid grid-cols-2 gap-3 sm:grid-cols-3 lg:grid-cols-6">
          {countdowns.map((c, index) => (
            <CountdownCard key={c.key} item={c} colorIndex={index} />
          ))}
        </div>
      </section>

      {/* Score cards + İlanlar (left column) alongside the sidebar */}
      <div className="mt-8 grid gap-6 lg:grid-cols-[1fr_20rem]">
        <div className="space-y-10">
          <section aria-labelledby="taban-puanlari">
          <SectionHeading id="taban-puanlari">Taban Puanları</SectionHeading>
          <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
            {scoreCards.map((card, index) => (
              <button
                key={card.key}
                onClick={() => onOpenScore(card)}
                className={`group relative flex min-h-32 flex-col justify-end overflow-hidden rounded-2xl border p-5 text-left transition-all hover:-translate-y-1 hover:shadow-lg ${['border-teal-200 bg-teal-50', 'border-navy-200 bg-navy-50', 'border-sky-200 bg-sky-50', 'border-cyan-200 bg-cyan-50', 'border-amber-200 bg-amber-50'][index % 5]}`}
              >
                <span
                  aria-hidden
                  className="absolute right-4 top-3 text-5xl font-bold leading-none text-navy-900/[0.06]"
                >
                  0{index + 1}
                </span>
                <span className="relative text-xs font-semibold uppercase tracking-wide text-teal-700">
                  {card.category}
                </span>
                <span className="relative mt-1 text-base font-bold text-navy-900">{card.title}</span>
                <span className="relative mt-3 inline-flex items-center gap-1 text-sm font-semibold text-teal-700 transition-all group-hover:gap-2">
                  İncele →
                </span>
              </button>
            ))}
          </div>
          </section>

          {/* İlanlar: güncel kamu personel alım ilanları (Taban Puanları ile aynı hizada) */}
          <section aria-labelledby="ilanlar-baslik">
        <div className="flex items-end justify-between">
          <SectionHeading id="ilanlar-baslik">Güncel İlanlar</SectionHeading>
          <button
            onClick={onOpenJobs}
            className="mb-3 inline-flex items-center gap-1 text-sm font-semibold text-teal-700 transition-all hover:gap-2"
          >
            Tüm ilanlar →
          </button>
        </div>
        <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
          {jobListings.slice(0, 6).map((job) => (
            <button
              key={job.id}
              onClick={onOpenJobs}
              className="group flex flex-col rounded-2xl border border-border bg-white p-5 text-left shadow-sm transition-all hover:-translate-y-1 hover:border-teal-300 hover:shadow-lg"
            >
              <div className="flex items-center gap-3">
                <span
                  aria-hidden
                  className="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-gradient-to-br from-navy-600 to-teal-600 text-xs font-bold text-white shadow-sm"
                >
                  {job.institution
                    .replace(/[^\p{L}\s]/gu, '')
                    .split(/\s+/)
                    .filter(Boolean)
                    .slice(0, 2)
                    .map((w) => w[0]?.toLocaleUpperCase('tr-TR'))
                    .join('')}
                </span>
                <span className="inline-flex rounded-full bg-teal-50 px-2.5 py-0.5 text-[11px] font-semibold text-teal-700 ring-1 ring-inset ring-teal-600/20">
                  {job.categoryLabel}
                </span>
              </div>
              <h3 className="mt-4 line-clamp-2 text-sm font-bold uppercase leading-snug text-navy-900 group-hover:text-teal-700">
                {job.institution}
              </h3>
              <p className="mt-2 flex-1 text-sm text-navy-600">{job.summary}</p>
              <div className="mt-4 flex items-center justify-between border-t border-border pt-3 text-xs">
                <span className="font-semibold italic text-amber-600">
                  {job.startDate} - {job.endDate}
                </span>
                <span className="font-semibold text-teal-700 opacity-0 transition-opacity group-hover:opacity-100">
                  İncele →
                </span>
              </div>
            </button>
          ))}
        </div>
          </section>
        </div>

        <aside className="space-y-6">
          <RecentPanel items={recentItems} />
          <AnnouncementsPanel items={announcements} />
        </aside>
      </div>
    </div>
  )
}
