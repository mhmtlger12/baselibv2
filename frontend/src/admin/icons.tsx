// Hafif, çizgisel ikon seti (stroke tabanlı). Panel genelinde kullanılır.
import type { SVGProps } from 'react'

type IconProps = SVGProps<SVGSVGElement>

function Base({ children, ...props }: IconProps & { children: React.ReactNode }) {
  return (
    <svg
      width="20"
      height="20"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="1.8"
      strokeLinecap="round"
      strokeLinejoin="round"
      aria-hidden
      {...props}
    >
      {children}
    </svg>
  )
}

export const IconDashboard = (p: IconProps) => (
  <Base {...p}>
    <path d="M3 12a9 9 0 0 1 18 0" />
    <path d="M12 12l4-3" />
    <circle cx="12" cy="12" r="1.4" fill="currentColor" stroke="none" />
    <path d="M3 12v5a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-5" />
  </Base>
)
export const IconUsers = (p: IconProps) => (
  <Base {...p}>
    <circle cx="9" cy="8" r="3" />
    <path d="M3.5 20a5.5 5.5 0 0 1 11 0" />
    <path d="M16 5.5a3 3 0 0 1 0 5.8M21 20a5.2 5.2 0 0 0-4-5" />
  </Base>
)
export const IconRoles = (p: IconProps) => (
  <Base {...p}>
    <path d="M12 3l7 3v5c0 4.2-2.9 7.6-7 9-4.1-1.4-7-4.8-7-9V6l7-3Z" />
    <path d="m9 12 2 2 4-4" />
  </Base>
)
export const IconKey = (p: IconProps) => (
  <Base {...p}>
    <circle cx="8" cy="15" r="3.5" />
    <path d="m10.5 12.5 8-8M16 5l2.5 2.5M13.5 7.5 16 10" />
  </Base>
)
export const IconTree = (p: IconProps) => (
  <Base {...p}>
    <rect x="9" y="3" width="6" height="4" rx="1" />
    <rect x="3" y="17" width="6" height="4" rx="1" />
    <rect x="15" y="17" width="6" height="4" rx="1" />
    <path d="M12 7v4M6 17v-2h12v2M12 15v-4" />
  </Base>
)
export const IconMenu = (p: IconProps) => (
  <Base {...p}>
    <path d="M8 6h13M8 12h13M8 18h13" />
    <circle cx="3.5" cy="6" r="1" fill="currentColor" stroke="none" />
    <circle cx="3.5" cy="12" r="1" fill="currentColor" stroke="none" />
    <circle cx="3.5" cy="18" r="1" fill="currentColor" stroke="none" />
  </Base>
)
export const IconGear = (p: IconProps) => (
  <Base {...p}>
    <circle cx="12" cy="12" r="3" />
    <path d="M12 2.5v2.5M12 19v2.5M4.5 4.5l1.8 1.8M17.7 17.7l1.8 1.8M2.5 12H5M19 12h2.5M4.5 19.5l1.8-1.8M17.7 6.3l1.8-1.8" />
  </Base>
)
export const IconActivity = (p: IconProps) => (
  <Base {...p}>
    <path d="M3 12h4l2.5 7 5-14L17 12h4" />
  </Base>
)
export const IconTrash = (p: IconProps) => (
  <Base {...p}>
    <path d="M4 7h16M9 7V5a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2M6 7l1 13a1 1 0 0 0 1 1h8a1 1 0 0 0 1-1l1-13M10 11v6M14 11v6" />
  </Base>
)
export const IconLogout = (p: IconProps) => (
  <Base {...p}>
    <path d="M14 4h4a1 1 0 0 1 1 1v14a1 1 0 0 1-1 1h-4M10 12H3M6 8l-3 4 3 4" />
  </Base>
)
export const IconEdit = (p: IconProps) => (
  <Base {...p}>
    <path d="M4 20h4L18.5 9.5a2 2 0 0 0-2.8-2.8L5 17v3ZM14.5 6.5l3 3" />
  </Base>
)
export const IconClose = (p: IconProps) => (
  <Base {...p}>
    <path d="M6 6l12 12M18 6 6 18" />
  </Base>
)
export const IconPlus = (p: IconProps) => (
  <Base {...p}>
    <path d="M12 5v14M5 12h14" />
  </Base>
)
export const IconSearch = (p: IconProps) => (
  <Base {...p}>
    <circle cx="11" cy="11" r="7" />
    <path d="m20 20-3.5-3.5" />
  </Base>
)
export const IconRestore = (p: IconProps) => (
  <Base {...p}>
    <path d="M4 12a8 8 0 1 1 2.6 5.9M4 12V7M4 12h5" />
  </Base>
)
export const IconChevron = (p: IconProps) => (
  <Base {...p}>
    <path d="m6 9 6 6 6-6" />
  </Base>
)
export const IconExternal = (p: IconProps) => (
  <Base {...p}>
    <path d="M14 4h6v6M20 4l-9 9M18 14v5a1 1 0 0 1-1 1H5a1 1 0 0 1-1-1V7a1 1 0 0 1 1-1h5" />
  </Base>
)
export const IconLayers = (p: IconProps) => (
  <Base {...p}>
    <path d="m12 3 8 4.5-8 4.5-8-4.5L12 3Z" />
    <path d="m4 12 8 4.5 8-4.5M4 16.5 12 21l8-4.5" />
  </Base>
)
export const IconImage = (p: IconProps) => (
  <Base {...p}>
    <rect x="3" y="4" width="18" height="16" rx="2" />
    <circle cx="8.5" cy="9" r="1.5" />
    <path d="m4 17 4.5-4.5 4 4L16 12l4 4" />
  </Base>
)
export const IconAd = (p: IconProps) => (
  <Base {...p}>
    <path d="M4 9v6h4l6 4V5L8 9H4Z" />
    <path d="M18 9a4 4 0 0 1 0 6" />
  </Base>
)
export const IconClock = (p: IconProps) => (
  <Base {...p}>
    <circle cx="12" cy="12" r="9" />
    <path d="M12 7v5l3 2" />
  </Base>
)
export const IconStar = (p: IconProps) => (
  <Base {...p}>
    <path d="m12 3 2.6 5.6 6 .7-4.4 4.1 1.2 5.9L12 16.9 6.6 19.3l1.2-5.9L3.4 9.3l6-.7L12 3Z" />
  </Base>
)
export const IconChat = (p: IconProps) => (
  <Base {...p}>
    <path d="M4 5h16a1 1 0 0 1 1 1v10a1 1 0 0 1-1 1H9l-4 4v-4H4a1 1 0 0 1-1-1V6a1 1 0 0 1 1-1Z" />
    <path d="M8 10h8M8 13h5" />
  </Base>
)
export const IconMail = (p: IconProps) => (
  <Base {...p}>
    <rect x="3" y="5" width="18" height="14" rx="2" />
    <path d="m4 7 8 6 8-6" />
  </Base>
)
export const IconLock = (p: IconProps) => (
  <Base {...p}>
    <rect x="5" y="11" width="14" height="9" rx="2" />
    <path d="M8 11V8a4 4 0 0 1 8 0v3" />
  </Base>
)
