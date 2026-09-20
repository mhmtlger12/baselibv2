// Site içerik yönetimi, veri odaklı (config-driven) çalışır. Her içerik türü;
// tablo kolonlarını, form alanlarını ve örnek verisini burada tanımlar. Gerçek
// veriler backend'den gelecektir; bu dosya yalnızca arayüzü besler.
import type { ReactNode } from 'react'

export type FieldType = 'text' | 'textarea' | 'number' | 'date' | 'image' | 'select' | 'checkbox'

export type FieldDef = {
  key: string
  label: string
  type: FieldType
  hint?: string
  options?: string[]
  full?: boolean // formda tam satır kaplasın
}

export type ColumnDef = {
  key: string
  label: string
  /** Hücreyi özel render eder (rozet, görsel önizleme vb.). */
  render?: (value: unknown, row: Row) => ReactNode
}

export type Row = Record<string, unknown> & { id: number }

export type ContentConfig = {
  title: string
  description: string
  addLabel: string
  columns: ColumnDef[]
  fields: FieldDef[]
  rows: Row[]
}

const activeColumn: ColumnDef = {
  key: 'active',
  label: 'Durum',
  render: (v) => <StatusBadge active={Boolean(v)} />,
}
const activeField: FieldDef = { key: 'active', label: 'Aktif', type: 'checkbox' }

function StatusBadge({ active }: { active: boolean }) {
  return (
    <span
      className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-semibold ring-1 ring-inset ${
        active ? 'bg-teal-100 text-teal-700 ring-teal-200' : 'bg-navy-50 text-navy-500 ring-navy-200'
      }`}
    >
      {active ? 'Aktif' : 'Pasif'}
    </span>
  )
}

const statusStyles: Record<string, string> = {
  Onaylı: 'bg-teal-100 text-teal-700 ring-teal-200',
  Yayında: 'bg-teal-100 text-teal-700 ring-teal-200',
  Okundu: 'bg-teal-100 text-teal-700 ring-teal-200',
  Beklemede: 'bg-amber-100 text-amber-700 ring-amber-200',
  'Yeni': 'bg-sky-100 text-sky-700 ring-sky-200',
  Spam: 'bg-rose-100 text-rose-700 ring-rose-200',
}

function StatusPill({ value }: { value: string }) {
  return (
    <span
      className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-semibold ring-1 ring-inset ${
        statusStyles[value] ?? 'bg-navy-50 text-navy-500 ring-navy-200'
      }`}
    >
      {value}
    </span>
  )
}

function Thumb({ src }: { src: string }) {
  return (
    <span className="flex h-11 w-16 items-center justify-center overflow-hidden rounded-lg bg-navy-50">
      {src ? (
        <img src={src} alt="" className="h-full w-full object-cover" />
      ) : (
        <span className="text-xs text-navy-300">—</span>
      )}
    </span>
  )
}

export const contentConfigs: Record<string, ContentConfig> = {
  'content/slider': {
    title: 'Slider Yönetimi',
    description: 'Ana sayfa üst alanındaki kayan görselleri (slider) yönetin.',
    addLabel: 'Yeni Slayt',
    columns: [
      { key: 'image', label: 'Görsel', render: (v) => <Thumb src={String(v)} /> },
      { key: 'title', label: 'Başlık' },
      { key: 'order', label: 'Sıra' },
      activeColumn,
    ],
    fields: [
      { key: 'title', label: 'Başlık', type: 'text', full: true },
      { key: 'description', label: 'Açıklama', type: 'textarea', full: true },
      { key: 'image', label: 'Görsel URL', type: 'image', full: true },
      { key: 'link', label: 'Bağlantı (URL)', type: 'text' },
      { key: 'order', label: 'Sıra', type: 'number' },
      activeField,
    ],
    rows: [
      { id: 1, title: '2024 KPSS Yerleştirme Sonuçları Açıklandı', description: 'Merkezi yerleştirme taban puanları güncellendi.', image: 'https://images.unsplash.com/photo-1523050854058-8df90110c9f1?w=400&h=250&fit=crop', link: '/kpss', order: 1, active: true },
      { id: 2, title: 'YKS Tercih Döneminde Doğru Karar', description: 'Üniversite ve bölüm bazında taban puanları.', image: 'https://images.unsplash.com/photo-1541339907198-e08756dedf3f?w=400&h=250&fit=crop', link: '/yks', order: 2, active: true },
      { id: 3, title: 'DGS ile Lisans Tamamlama Fırsatları', description: 'Önlisans mezunları için geçiş bölümleri.', image: 'https://images.unsplash.com/photo-1592280771190-3e2e4d571952?w=400&h=250&fit=crop', link: '/dgs', order: 3, active: false },
    ],
  },

  'content/ads': {
    title: 'Reklam Alanları',
    description: 'Sitedeki reklam alanlarını, konumlarını ve içeriklerini yönetin.',
    addLabel: 'Yeni Reklam',
    columns: [
      { key: 'name', label: 'Alan Adı' },
      { key: 'position', label: 'Konum' },
      { key: 'image', label: 'Görsel', render: (v) => <Thumb src={String(v)} /> },
      activeColumn,
    ],
    fields: [
      { key: 'name', label: 'Alan Adı', type: 'text' },
      // Konumlar sitede fiilen render edilen reklam alanlarıyla birebir eşleşir.
      { key: 'position', label: 'Konum', type: 'select', options: ['Anasayfa Üst (Slider Yanı)', 'Sidebar Üstü'] },
      { key: 'image', label: 'Görsel URL', type: 'image', full: true },
      { key: 'link', label: 'Yönlendirme (URL)', type: 'text', full: true },
      activeField,
    ],
    rows: [
      { id: 1, name: 'Anasayfa Üst Banner', position: 'Anasayfa Üst (Slider Yanı)', image: 'https://images.unsplash.com/photo-1557804506-669a67965ba0?w=400&h=250&fit=crop', link: 'https://reklam.example.com', active: true },
      { id: 2, name: 'Sidebar Reklamı', position: 'Sidebar Üstü', image: '', link: 'https://reklam.example.com', active: true },
    ],
  },

  'content/countdowns': {
    title: 'Sınav Sayaçları',
    description: 'Ana sayfadaki sınav geri sayım sayaçlarını yönetin.',
    addLabel: 'Yeni Sayaç',
    columns: [
      { key: 'label', label: 'Sınav' },
      { key: 'target', label: 'Tarih' },
      activeColumn,
    ],
    fields: [
      { key: 'label', label: 'Sınav Adı', type: 'text' },
      { key: 'target', label: 'Sınav Tarihi', type: 'date' },
      activeField,
    ],
    rows: [
      { id: 1, label: 'YKS', target: '2026-06-20', active: true },
      { id: 2, label: 'KPSS Lisans', target: '2026-07-19', active: true },
      { id: 3, label: 'DGS', target: '2026-07-05', active: true },
      { id: 4, label: 'ALES', target: '2026-05-10', active: true },
    ],
  },

  'content/sidebar-recent': {
    title: 'Son Eklenenler Sidebar',
    description: '"Son Eklenenler" sidebar bloğundaki öğeleri yönetin.',
    addLabel: 'Yeni Öğe',
    columns: [
      { key: 'title', label: 'Başlık' },
      { key: 'category', label: 'Kategori' },
      { key: 'order', label: 'Sıra' },
      activeColumn,
    ],
    fields: [
      { key: 'title', label: 'Başlık', type: 'text', full: true },
      { key: 'category', label: 'Kategori', type: 'select', options: ['KPSS', 'YKS', 'ALES', 'DGS'] },
      { key: 'link', label: 'Bağlantı (URL)', type: 'text', full: true },
      { key: 'order', label: 'Sıra', type: 'number' },
      activeField,
    ],
    rows: [
      { id: 1, title: 'Bilgisayar Mühendisliği 2024 Taban Puanı', category: 'YKS', link: '/yks/bilgisayar-muh', order: 1, active: true },
      { id: 2, title: 'KPSS Önlisans Atama Puanları', category: 'KPSS', link: '/kpss/onlisans', order: 2, active: true },
      { id: 3, title: 'Hemşirelik DGS Geçiş Puanları', category: 'DGS', link: '/dgs/hemsirelik', order: 3, active: true },
    ],
  },

  'content/sidebar-osym': {
    title: 'ÖSYM Duyuruları Sidebar',
    description: '"ÖSYM Duyuruları" sidebar bloğundaki duyuruları yönetin.',
    addLabel: 'Yeni Duyuru',
    columns: [
      { key: 'title', label: 'Duyuru' },
      { key: 'date', label: 'Tarih' },
      activeColumn,
    ],
    fields: [
      { key: 'title', label: 'Duyuru Başlığı', type: 'text', full: true },
      { key: 'date', label: 'Yayın Tarihi', type: 'date' },
      { key: 'link', label: 'ÖSYM Bağlantısı (URL)', type: 'text', full: true },
      activeField,
    ],
    rows: [
      { id: 1, title: '2026 YKS Başvuru Kılavuzu Yayımlandı', date: '2026-02-10', link: 'https://osym.gov.tr', active: true },
      { id: 2, title: 'KPSS 2026 Sınav Takvimi Açıklandı', date: '2026-01-15', link: 'https://osym.gov.tr', active: true },
      { id: 3, title: 'DGS Tercih İşlemleri Başladı', date: '2026-07-22', link: 'https://osym.gov.tr', active: false },
    ],
  },

  'content/score-cards': {
    title: 'Taban Puanı Kategori Kartları',
    description: 'Ana sayfadaki "Taban Puanları" bölümündeki tıklanabilir kategori kartlarını yönetin.',
    addLabel: 'Yeni Kart',
    columns: [
      { key: 'order', label: 'Sıra' },
      { key: 'title', label: 'Kart Başlığı' },
      { key: 'category', label: 'Etiket' },
      { key: 'href', label: 'Bağlantı' },
      activeColumn,
    ],
    fields: [
      { key: 'title', label: 'Kart Başlığı', type: 'text', full: true },
      { key: 'category', label: 'Üst Etiket', type: 'text' },
      { key: 'href', label: 'Bağlantı (slug)', type: 'text' },
      { key: 'order', label: 'Sıra', type: 'number' },
      activeField,
    ],
    rows: [
      { id: 1, order: 1, title: 'KPSS Lisans', category: 'Taban Puanı', href: 'kpss-lisans', active: true },
      { id: 2, order: 2, title: 'KPSS Önlisans', category: 'Taban Puanı', href: 'kpss-onlisans', active: true },
      { id: 3, order: 3, title: 'KPSS Ortaöğretim', category: 'Taban Puanı', href: 'kpss-orta', active: true },
      { id: 4, order: 4, title: 'YKS', category: 'Taban Puanı', href: 'yks', active: true },
      { id: 5, order: 5, title: 'DGS', category: 'Taban Puanı', href: 'dgs', active: true },
    ],
  },

  'content/departments': {
    title: 'Bölümler',
    description: 'Sınav seviyelerine göre listelenen bölümleri (Lisans / Önlisans / Ortaöğretim) yönetin.',
    addLabel: 'Yeni Bölüm',
    columns: [
      { key: 'name', label: 'Bölüm Adı' },
      { key: 'level', label: 'Seviye', render: (v) => <StatusPill value={String(v)} /> },
      activeColumn,
    ],
    fields: [
      { key: 'name', label: 'Bölüm Adı', type: 'text', full: true },
      { key: 'level', label: 'Sınav Seviyesi', type: 'select', options: ['Lisans', 'Önlisans', 'Ortaöğretim'] },
      activeField,
    ],
    rows: [
      { id: 1, name: 'Bilgisayar Mühendisliği', level: 'Lisans', active: true },
      { id: 2, name: 'Hukuk', level: 'Lisans', active: true },
      { id: 3, name: 'Hemşirelik', level: 'Lisans', active: true },
      { id: 4, name: 'Bilgisayar Programcılığı', level: 'Önlisans', active: true },
      { id: 5, name: 'Adalet', level: 'Önlisans', active: true },
      { id: 6, name: 'Zabıt Katipliği', level: 'Ortaöğretim', active: true },
      { id: 7, name: 'Veri Hazırlama ve Kontrol İşletmeni', level: 'Ortaöğretim', active: false },
    ],
  },

  'content/comments': {
    title: 'Yorum Moderasyonu',
    description: 'Kullanıcı yorumlarını onaylayın, bekletin veya spam olarak işaretleyin.',
    addLabel: 'Yeni Yorum',
    columns: [
      { key: 'author', label: 'Yazar' },
      { key: 'body', label: 'Yorum' },
      { key: 'page', label: 'Sayfa' },
      { key: 'status', label: 'Durum', render: (v) => <StatusPill value={String(v)} /> },
      { key: 'date', label: 'Tarih' },
    ],
    fields: [
      { key: 'author', label: 'Yazar', type: 'text' },
      { key: 'status', label: 'Durum', type: 'select', options: ['Onaylı', 'Beklemede', 'Spam'] },
      { key: 'body', label: 'Yorum Metni', type: 'textarea', full: true },
      { key: 'page', label: 'İlgili Sayfa', type: 'text', full: true },
      { key: 'date', label: 'Tarih', type: 'text' },
    ],
    rows: [
      { id: 1, author: 'Ahmet Y.', body: 'KPSS lisans için bu puanlar yeterli mi?', page: 'Bilgisayar Mühendisliği', status: 'Onaylı', date: '2026-09-18' },
      { id: 2, author: 'Zeynep D.', body: 'Sağlık Bakanlığı kadrolarında taban puan daha düşük görünüyor.', page: 'KPSS Lisans', status: 'Beklemede', date: '2026-09-19' },
      { id: 3, author: 'guest_4821', body: 'ucuz takipçi -> bit.ly/xxx', page: 'YKS', status: 'Spam', date: '2026-09-20' },
    ],
  },

  'content/messages': {
    title: 'İletişim Mesajları',
    description: 'İletişim formundan gelen mesajları görüntüleyin ve durumlarını yönetin.',
    addLabel: 'Yeni Mesaj',
    columns: [
      { key: 'name', label: 'Ad Soyad' },
      { key: 'email', label: 'E-posta' },
      { key: 'subject', label: 'Konu' },
      { key: 'status', label: 'Durum', render: (v) => <StatusPill value={String(v)} /> },
      { key: 'date', label: 'Tarih' },
    ],
    fields: [
      { key: 'name', label: 'Ad Soyad', type: 'text' },
      { key: 'email', label: 'E-posta', type: 'text' },
      { key: 'subject', label: 'Konu', type: 'text', full: true },
      { key: 'message', label: 'Mesaj', type: 'textarea', full: true },
      { key: 'status', label: 'Durum', type: 'select', options: ['Yeni', 'Okundu'] },
      { key: 'date', label: 'Tarih', type: 'text' },
    ],
    rows: [
      { id: 1, name: 'Elif Şahin', email: 'elif@example.com', subject: 'Puan hesaplama hakkında', message: 'Merhaba, KPSS puan hesaplamasında...', status: 'Yeni', date: '2026-09-20' },
      { id: 2, name: 'Burak Demir', email: 'burak@example.com', subject: 'Reklam iş birliği', message: 'Sitenizde reklam vermek istiyorum.', status: 'Okundu', date: '2026-09-17' },
    ],
  },

  'content/jobs': {
    title: 'İlanlar',
    description: 'Kamu personel alım ilanlarını (memur, sözleşmeli, akademik, işçi vb.) yönetin.',
    addLabel: 'Yeni İlan',
    columns: [
      { key: 'institution', label: 'Kurum / Kuruluş' },
      { key: 'summary', label: 'İlan Özeti' },
      { key: 'categoryLabel', label: 'Kategori', render: (v) => <StatusPill value={String(v)} /> },
      { key: 'publishedAt', label: 'Yayın' },
      { key: 'range', label: 'Başvuru', render: (_v, r) => `${r.startDate} - ${r.endDate}` },
      activeColumn,
    ],
    fields: [
      { key: 'institution', label: 'Kurum / Kuruluş Adı', type: 'text', full: true },
      { key: 'summary', label: 'İlan Özeti', type: 'text', full: true, hint: 'Örn: 25 Öğretim Elemanı Alacak' },
      {
        key: 'categoryLabel',
        label: 'Kategori',
        type: 'select',
        options: [
          'A Grubu Memur (Kariyer Meslek)',
          'B Grubu Memur',
          'Kariyer Sözleşmeli Personel',
          '4/B Sözleşmeli Personel',
          'KİT Sözleşmeli Personel',
          'Kurumsal Sözleşmeli Personel',
          'Öğretim Üyesi',
          'Öğretim Görevlisi',
          'Araştırma Görevlisi',
          'Sürekli İşçi',
          'Geçici İşçi',
          'Askeri Personel',
          'Yargı Mensubu (Hakim - Savcı)',
        ],
      },
      { key: 'publishedAt', label: 'Yayın Tarihi', type: 'date' },
      { key: 'startDate', label: 'Başvuru Başlangıcı', type: 'text', hint: 'Örn: 20 Eylül' },
      { key: 'endDate', label: 'Başvuru Bitişi', type: 'text', hint: 'Örn: 4 Ekim' },
      activeField,
    ],
    rows: [
      { id: 1, institution: 'Adana Alparslan Türkeş Bilim ve Teknoloji Üniversitesi', summary: '25 Öğretim Elemanı Alacak', categoryLabel: 'Öğretim Üyesi', publishedAt: '2026-09-20', startDate: '20 Eylül', endDate: '4 Ekim', active: true },
      { id: 2, institution: 'İstanbul Teknik Üniversitesi', summary: '44 Öğretim Üyesi Alacak', categoryLabel: 'Öğretim Üyesi', publishedAt: '2026-09-18', startDate: '18 Eylül', endDate: '2 Ekim', active: true },
      { id: 3, institution: 'Gençlik ve Spor Bakanlığı', summary: '9 Bilişim Personeli Alacak', categoryLabel: '4/B Sözleşmeli Personel', publishedAt: '2026-09-18', startDate: '21 Eylül', endDate: '25 Eylül', active: true },
      { id: 4, institution: 'Adalet Bakanlığı', summary: '250 Zabıt Kâtibi (B Grubu) Alacak', categoryLabel: 'B Grubu Memur', publishedAt: '2026-09-16', startDate: '16 Eylül', endDate: '29 Eylül', active: true },
      { id: 5, institution: 'Hâkimler ve Savcılar Kurulu', summary: '150 Adli Yargı Hâkim Adayı Alacak', categoryLabel: 'Yargı Mensubu (Hakim - Savcı)', publishedAt: '2026-09-15', startDate: '15 Eylül', endDate: '3 Ekim', active: false },
    ],
  },

  'content/job-categories': {
    title: 'İlan Kategorileri',
    description: 'İlanlar sayfasının sol menüsündeki kategori ağacını yönetin.',
    addLabel: 'Yeni Kategori',
    columns: [
      { key: 'label', label: 'Kategori' },
      { key: 'parent', label: 'Üst Kategori' },
      { key: 'order', label: 'Sıra' },
      activeColumn,
    ],
    fields: [
      { key: 'label', label: 'Kategori Adı', type: 'text', full: true },
      {
        key: 'parent',
        label: 'Üst Kategori',
        type: 'select',
        options: ['—', 'Memur', 'Sözleşmeli Personel', 'Akademik Personel', 'İşçi'],
      },
      { key: 'order', label: 'Sıra', type: 'number' },
      activeField,
    ],
    rows: [
      { id: 1, label: 'Memur', parent: '—', order: 1, active: true },
      { id: 2, label: 'A Grubu Memur (Kariyer Meslek)', parent: 'Memur', order: 1, active: true },
      { id: 3, label: 'B Grubu Memur', parent: 'Memur', order: 2, active: true },
      { id: 4, label: 'Akademik Personel', parent: '—', order: 2, active: true },
      { id: 5, label: 'Öğretim Üyesi', parent: 'Akademik Personel', order: 1, active: true },
      { id: 6, label: 'Askeri Personel', parent: '—', order: 3, active: true },
    ],
  },

  'content/taban-kpss': makeScoreConfig('KPSS'),
  'content/taban-dgs': makeScoreConfig('DGS'),
  'content/taban-yks': makeScoreConfig('YKS'),

}

function makeScoreConfig(exam: string): ContentConfig {
  const sampleByExam: Record<string, Row[]> = {
    KPSS: [
      { id: 1, program: 'Bilgisayar Programcılığı', institution: 'Sağlık Bakanlığı', score: 88.42, rank: 1240, year: 2024, active: true },
      { id: 2, program: 'Büro Yönetimi', institution: 'Adalet Bakanlığı', score: 82.15, rank: 3980, year: 2024, active: true },
    ],
    DGS: [
      { id: 1, program: 'Bilgisayar Mühendisliği', institution: 'İTÜ', score: 342.8, rank: 210, year: 2024, active: true },
      { id: 2, program: 'Hemşirelik', institution: 'Ege Üniversitesi', score: 318.5, rank: 640, year: 2024, active: true },
    ],
    YKS: [
      { id: 1, program: 'Tıp', institution: 'Hacettepe Üniversitesi', score: 541.2, rank: 850, year: 2024, active: true },
      { id: 2, program: 'Hukuk', institution: 'Ankara Üniversitesi', score: 498.6, rank: 5400, year: 2024, active: true },
    ],
  }
  return {
    title: `Taban Puanları · ${exam}`,
    description: `${exam} taban puanları ve başarı sıralaması kayıtlarını yönetin.`,
    addLabel: 'Yeni Kayıt',
    columns: [
      { key: 'program', label: 'Program / Bölüm' },
      { key: 'institution', label: 'Kurum / Üniversite' },
      { key: 'score', label: 'Taban Puanı' },
      { key: 'rank', label: 'Sıralama' },
      { key: 'year', label: 'Yıl' },
      activeColumn,
    ],
    fields: [
      { key: 'program', label: 'Program / Bölüm', type: 'text', full: true },
      { key: 'institution', label: 'Kurum / Üniversite', type: 'text', full: true },
      { key: 'score', label: 'Taban Puanı', type: 'number' },
      { key: 'rank', label: 'Başarı Sıralaması', type: 'number' },
      { key: 'year', label: 'Yıl', type: 'number' },
      activeField,
    ],
    rows: sampleByExam[exam],
  }
}
