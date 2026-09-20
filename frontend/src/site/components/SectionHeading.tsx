export default function SectionHeading({ id, children }: { id?: string; children: React.ReactNode }) {
  return (
    <h2 id={id} className="mb-4 flex items-center gap-2.5 text-lg font-bold text-navy-900">
      <span aria-hidden className="h-5 w-1.5 rounded-full bg-gradient-to-b from-teal-400 to-navy-700" />
      {children}
    </h2>
  )
}
