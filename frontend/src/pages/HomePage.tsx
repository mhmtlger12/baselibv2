import Slider from '../components/Slider'
import CountdownCard from '../components/CountdownCard'
import SectionHeading from '../components/SectionHeading'
import { AdBox, AnnouncementsPanel, RecentPanel } from '../components/SidebarBlocks'
import {
  announcements,
  countdowns,
  popularUniversities,
  recentItems,
  scoreCards,
  slides,
  type ScoreCard,
} from '../data/site'

type HomePageProps = {
  onOpenScore: (card: ScoreCard) => void
}

export default function HomePage({ onOpenScore }: HomePageProps) {
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

      {/* Score cards + sidebar, aligned on the same row */}
      <div className="mt-8 grid gap-6 lg:grid-cols-[1fr_20rem]">
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

        <aside className="space-y-6">
          <RecentPanel items={recentItems} />
          <AnnouncementsPanel items={announcements} />
        </aside>
      </div>

      {/* Popular universities */}
      <section aria-labelledby="populer-uni" className="mt-10">
        <SectionHeading id="populer-uni">Popüler Üniversiteler</SectionHeading>
        <ul className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-5">
          {popularUniversities.map((uni, index) => (
            <li key={uni.id}>
              <a
                href="#"
                className={`group relative flex min-h-32 flex-col justify-end overflow-hidden rounded-2xl border p-5 transition-all hover:-translate-y-1 hover:shadow-lg ${['border-teal-200 bg-teal-50', 'border-navy-200 bg-navy-50', 'border-sky-200 bg-sky-50', 'border-cyan-200 bg-cyan-50', 'border-amber-200 bg-amber-50'][index % 5]}`}
              >
                <span aria-hidden className="absolute right-4 top-3 text-5xl font-bold leading-none text-navy-900/[0.06]">0{index + 1}</span>
                <span className="relative text-base font-bold leading-snug text-navy-900">{uni.name}</span>
                <span className="relative mt-2 inline-flex w-fit rounded-full bg-white/80 px-2.5 py-1 text-xs font-medium text-navy-700 shadow-sm">{uni.city}</span>
                <span className="relative mt-3 text-xs font-semibold text-teal-700 opacity-0 transition-opacity group-hover:opacity-100">Puanları incele →</span>
              </a>
            </li>
          ))}
        </ul>
      </section>
    </div>
  )
}
