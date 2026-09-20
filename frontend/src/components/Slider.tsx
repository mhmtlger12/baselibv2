import { useEffect, useState } from 'react'
import type { Slide } from '../data/site'

export default function Slider({ slides }: { slides: Slide[] }) {
  const [index, setIndex] = useState(0)

  useEffect(() => {
    const id = setInterval(() => setIndex((i) => (i + 1) % slides.length), 6000)
    return () => clearInterval(id)
  }, [slides.length])

  const slide = slides[index]

  return (
    <div className="relative h-64 overflow-hidden rounded-lg border border-border bg-navy-100 sm:h-72">
      <img
        src={slide.image}
        alt={slide.title}
        className="absolute inset-0 h-full w-full object-cover"
      />
      <div className="absolute inset-0 bg-gradient-to-t from-navy-900/85 via-navy-900/40 to-transparent" />
      <div className="absolute inset-x-0 bottom-0 p-6 text-white">
        <h2 className="max-w-lg text-xl font-bold sm:text-2xl">{slide.title}</h2>
        <p className="mt-2 max-w-lg text-sm text-navy-100">{slide.description}</p>
      </div>
      <div className="absolute bottom-4 right-4 flex gap-2">
        {slides.map((s, i) => (
          <button
            key={s.id}
            onClick={() => setIndex(i)}
            aria-label={`${i + 1}. slayta git`}
            aria-current={i === index}
            className={`h-2 rounded-full transition-all ${
              i === index ? 'w-6 bg-white' : 'w-2 bg-white/50'
            }`}
          />
        ))}
      </div>
    </div>
  )
}
