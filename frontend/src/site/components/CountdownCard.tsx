import { useEffect, useState } from 'react'
import type { Countdown } from '../data/site'

function daysUntil(target: string): number {
  const diff = new Date(target).getTime() - Date.now()
  return Math.max(0, Math.ceil(diff / (1000 * 60 * 60 * 24)))
}

type CountdownCardProps = {
  item: Countdown
  emphasis?: boolean
  colorIndex?: number
}

export default function CountdownCard({ item, emphasis = false, colorIndex = 0 }: CountdownCardProps) {
  const [days, setDays] = useState(() => daysUntil(item.target))
  const pastelColors = [
    'border-teal-200 bg-teal-50',
    'border-navy-200 bg-navy-50',
    'border-sky-200 bg-sky-50',
    'border-cyan-200 bg-cyan-50',
    'border-amber-200 bg-amber-50',
  ]

  useEffect(() => {
    const id = setInterval(() => setDays(daysUntil(item.target)), 60 * 1000)
    return () => clearInterval(id)
  }, [item.target])

  return (
    <div
      className={`group relative flex flex-col justify-between overflow-hidden rounded-xl border p-4 transition-all hover:-translate-y-0.5 hover:shadow-md ${
        emphasis
          ? 'border-navy-600 bg-navy-700 text-white'
          : `${pastelColors[colorIndex % pastelColors.length]} text-navy-900`
      }`}
    >
      <span
        aria-hidden
        className="absolute inset-x-0 top-0 h-1 bg-gradient-to-r from-teal-400 to-navy-600"
      />
      <p className={`text-sm font-semibold ${emphasis ? 'text-navy-100' : 'text-navy-800'}`}>
        {item.label}
      </p>
      <p className="mt-3 flex items-baseline gap-1">
        <span className="text-3xl font-bold tabular-nums tracking-tight">{days}</span>
        <span className={`text-sm ${emphasis ? 'text-navy-100' : 'text-navy-700'}`}>
          gün kaldı
        </span>
      </p>
    </div>
  )
}
