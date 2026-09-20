import { SITE_NAME, SITE_TAGLINE } from '../data/site'

const scoreLinks = ['KPSS Taban Puanları', 'YKS Taban Puanları', 'ALES Taban Puanları', 'DGS Taban Puanları']
const siteLinks = ['Hakkımızda', 'Gizlilik Politikası', 'İletişim', 'Kullanım Koşulları']

function ArrowLink({ children }: { children: React.ReactNode }) {
  return (
    <a href="#" className="group flex items-center gap-2 border-b border-white/10 py-2.5 text-sm text-navy-200 transition-colors hover:text-white">
      <span aria-hidden className="text-teal-400 transition-transform group-hover:translate-x-0.5">→</span>
      {children}
    </a>
  )
}

export default function Footer() {
  return (
    <footer className="mt-14 border-t-2 border-teal-400 bg-navy-900 text-navy-100">
      <div className="mx-auto grid max-w-6xl gap-10 px-4 py-11 sm:px-6 lg:grid-cols-[1.15fr_1.7fr] lg:gap-16">
        <section aria-label="puannokta hakkında">
          <div className="flex items-center gap-3">
            <span aria-hidden className="flex h-11 w-11 items-center justify-center rounded-xl bg-teal-500 text-lg font-bold text-white shadow-lg shadow-teal-950/30">p.</span>
            <div>
              <p className="font-bold tracking-tight text-white">{SITE_NAME}</p>
              <p className="text-xs text-navy-300">{SITE_TAGLINE}</p>
            </div>
          </div>
          <div className="mt-5 h-px w-12 bg-teal-400" />
          <p className="mt-3 max-w-sm text-sm leading-6 text-navy-200">KPSS, YKS, ALES ve DGS için güncel taban puanlarını güvenle karşılaştır, tercihine yön ver.</p>
          <div className="mt-5 flex gap-2" aria-label="Sosyal medya bağlantıları">
            {['f', '𝕏', '▶', '◎'].map((icon, index) => (
              <a key={icon} href="#" aria-label={['Facebook', 'X', 'YouTube', 'Instagram'][index]} className="flex h-9 w-9 items-center justify-center rounded-lg bg-white/10 text-sm font-bold text-white transition hover:-translate-y-0.5 hover:bg-teal-500">{icon}</a>
            ))}
          </div>
        </section>

        <nav aria-label="Footer bağlantıları" className="grid gap-8 sm:grid-cols-2">
          <div>
            <h2 className="text-sm font-semibold text-white">Puan Kategorileri</h2>
            <div className="mt-4 border-t border-white/10">{scoreLinks.map((link) => <ArrowLink key={link}>{link}</ArrowLink>)}</div>
          </div>
          <div>
            <h2 className="text-sm font-semibold text-white">Site Hakkında</h2>
            <div className="mt-4 border-t border-white/10">{siteLinks.map((link) => <ArrowLink key={link}>{link}</ArrowLink>)}</div>
          </div>
        </nav>
      </div>
      <div className="border-t border-white/10">
        <div className="mx-auto flex max-w-6xl flex-col gap-2 px-4 py-4 text-xs text-navy-300 sm:flex-row sm:items-center sm:justify-between sm:px-6">
          <p>© {new Date().getFullYear()} {SITE_NAME}. Tüm hakları saklıdır.</p>
          <p>Veriler ÖSYM ve kurum duyurularından derlenir.</p>
        </div>
      </div>
    </footer>
  )
}
