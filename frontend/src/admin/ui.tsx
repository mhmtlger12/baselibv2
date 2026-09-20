// Panel genelinde paylaşılan arayüz parçaları.
import { useEffect, type ReactNode } from 'react'
import { IconClose, IconSearch } from './icons'

/** Beyaz kart yüzeyi. */
export function Panel({ children, className = '' }: { children: ReactNode; className?: string }) {
  return (
    <div
      className={`rounded-2xl border border-navy-100 bg-white shadow-[0_1px_2px_rgba(16,29,58,0.04),0_12px_32px_-24px_rgba(16,29,58,0.35)] ${className}`}
    >
      {children}
    </div>
  )
}

/** Bölüm başlığı + açıklama + sağ aksiyon alanı. */
export function SectionHead({
  title,
  description,
  action,
}: {
  title: string
  description: string
  action?: ReactNode
}) {
  return (
    <div className="mb-5 flex flex-wrap items-start justify-between gap-4">
      <div>
        <h2 className="text-xl font-bold tracking-tight text-navy-900">{title}</h2>
        <p className="mt-1 max-w-2xl text-sm text-muted-foreground">{description}</p>
      </div>
      {action}
    </div>
  )
}

type BadgeTone = 'active' | 'passive' | 'admin' | 'info' | 'warning' | 'danger'

const badgeTones: Record<BadgeTone, string> = {
  active: 'bg-teal-100 text-teal-700 ring-teal-200',
  passive: 'bg-navy-50 text-navy-500 ring-navy-200',
  admin: 'bg-navy-100 text-navy-700 ring-navy-200',
  info: 'bg-sky-100 text-sky-700 ring-sky-200',
  warning: 'bg-amber-100 text-amber-700 ring-amber-200',
  danger: 'bg-rose-100 text-rose-700 ring-rose-200',
}

export function Badge({ tone = 'info', children }: { tone?: BadgeTone; children: ReactNode }) {
  return (
    <span
      className={`inline-flex items-center gap-1 rounded-full px-2.5 py-0.5 text-xs font-semibold ring-1 ring-inset ${badgeTones[tone]}`}
    >
      {children}
    </span>
  )
}

type BtnVariant = 'primary' | 'ghost' | 'soft' | 'danger'

const btnVariants: Record<BtnVariant, string> = {
  primary:
    'bg-navy-700 text-white hover:bg-navy-800 shadow-sm shadow-navy-900/10',
  ghost: 'text-navy-600 hover:bg-navy-50',
  soft: 'bg-navy-50 text-navy-700 hover:bg-navy-100',
  danger: 'bg-rose-50 text-rose-600 hover:bg-rose-100',
}

export function Button({
  variant = 'primary',
  className = '',
  children,
  ...props
}: { variant?: BtnVariant } & React.ButtonHTMLAttributes<HTMLButtonElement>) {
  return (
    <button
      className={`inline-flex items-center justify-center gap-2 rounded-xl px-4 py-2 text-sm font-semibold transition-colors ${btnVariants[variant]} ${className}`}
      {...props}
    >
      {children}
    </button>
  )
}

/** Küçük, kare aksiyon butonu (tablolarda satır işlemleri). */
export function IconButton({
  tone = 'edit',
  className = '',
  children,
  ...props
}: { tone?: 'edit' | 'delete' | 'plain' } & React.ButtonHTMLAttributes<HTMLButtonElement>) {
  const tones = {
    edit: 'bg-amber-100 text-amber-700 hover:bg-amber-200',
    delete: 'bg-rose-100 text-rose-600 hover:bg-rose-200',
    plain: 'bg-navy-50 text-navy-600 hover:bg-navy-100',
  }
  return (
    <button
      className={`inline-flex h-9 w-9 items-center justify-center rounded-lg transition-colors ${tones[tone]} ${className}`}
      {...props}
    >
      {children}
    </button>
  )
}

export function SearchInput({
  value,
  onChange,
  placeholder = 'Ara...',
}: {
  value: string
  onChange: (v: string) => void
  placeholder?: string
}) {
  return (
    <div className="flex w-full max-w-xs items-center gap-2 rounded-xl border border-navy-100 bg-navy-50/60 px-3 py-2 transition-colors focus-within:border-teal-400 focus-within:bg-white">
      <IconSearch className="h-4 w-4 text-navy-400" />
      <input
        value={value}
        onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder}
        className="w-full bg-transparent text-sm text-navy-900 outline-none placeholder:text-navy-400"
      />
    </div>
  )
}

/** Form alanı sarmalayıcı. */
export function Field({
  label,
  hint,
  children,
}: {
  label: string
  hint?: ReactNode
  children: ReactNode
}) {
  return (
    <label className="block">
      <span className="mb-1.5 block text-sm font-medium text-navy-700">{label}</span>
      {children}
      {hint && <span className="mt-1 block text-xs text-muted-foreground">{hint}</span>}
    </label>
  )
}

const inputBase =
  'w-full rounded-xl border border-navy-100 bg-white px-3.5 py-2.5 text-sm text-navy-900 outline-none transition-colors placeholder:text-navy-300 focus:border-teal-400 focus:ring-2 focus:ring-teal-100'

export function TextInput(props: React.InputHTMLAttributes<HTMLInputElement>) {
  return <input {...props} className={`${inputBase} ${props.className ?? ''}`} />
}

export function Select(props: React.SelectHTMLAttributes<HTMLSelectElement>) {
  return <select {...props} className={`${inputBase} appearance-none ${props.className ?? ''}`} />
}

export function Textarea(props: React.TextareaHTMLAttributes<HTMLTextAreaElement>) {
  return <textarea {...props} className={`${inputBase} min-h-[84px] resize-y ${props.className ?? ''}`} />
}

/** Etiketli anahtar (checkbox). */
export function Check({
  checked,
  onChange,
  label,
}: {
  checked: boolean
  onChange: (v: boolean) => void
  label: ReactNode
}) {
  return (
    <label className="inline-flex cursor-pointer items-center gap-2 text-sm text-navy-700">
      <span
        onClick={() => onChange(!checked)}
        className={`flex h-5 w-5 items-center justify-center rounded-md border transition-colors ${
          checked ? 'border-teal-500 bg-teal-500 text-white' : 'border-navy-200 bg-white'
        }`}
      >
        {checked && (
          <svg width="12" height="12" viewBox="0 0 24 24" fill="none" aria-hidden>
            <path d="m5 13 4 4 10-10" stroke="currentColor" strokeWidth="3" strokeLinecap="round" strokeLinejoin="round" />
          </svg>
        )}
      </span>
      {label}
    </label>
  )
}

/** Modal. Escape ve arka plan tıklaması ile kapanır. */
export function Modal({
  title,
  onClose,
  children,
  footer,
  wide = false,
}: {
  title: string
  onClose: () => void
  children: ReactNode
  footer?: ReactNode
  wide?: boolean
}) {
  useEffect(() => {
    const onKey = (e: KeyboardEvent) => e.key === 'Escape' && onClose()
    document.addEventListener('keydown', onKey)
    document.body.style.overflow = 'hidden'
    return () => {
      document.removeEventListener('keydown', onKey)
      document.body.style.overflow = ''
    }
  }, [onClose])

  return (
    <div className="fixed inset-0 z-50 flex items-start justify-center overflow-y-auto bg-navy-900/50 p-4 backdrop-blur-sm sm:p-8">
      <div
        role="dialog"
        aria-modal
        aria-label={title}
        className={`my-auto w-full ${wide ? 'max-w-4xl' : 'max-w-xl'} rounded-2xl border border-navy-100 bg-white shadow-2xl`}
      >
        <div className="flex items-center justify-between border-b border-navy-100 px-6 py-4">
          <h3 className="text-lg font-bold text-navy-900">{title}</h3>
          <button
            onClick={onClose}
            aria-label="Kapat"
            className="rounded-lg p-1.5 text-navy-400 transition-colors hover:bg-navy-50 hover:text-navy-700"
          >
            <IconClose className="h-5 w-5" />
          </button>
        </div>
        <div className="px-6 py-5">{children}</div>
        {footer && (
          <div className="flex items-center justify-end gap-2 border-t border-navy-100 px-6 py-4">
            {footer}
          </div>
        )}
      </div>
    </div>
  )
}
