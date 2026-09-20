export const SITE_NAME = 'puannokta'
export const SITE_TAGLINE = 'Taban puanları, tek noktada.'

export const navLinks = [
  { key: 'home', label: 'Ana Sayfa' },
  { key: 'ilanlar', label: 'İlanlar' },
  { key: 'kpss', label: 'KPSS' },
  { key: 'yks', label: 'YKS' },
  { key: 'ales', label: 'ALES' },
  { key: 'dgs', label: 'DGS' },
  { key: 'iletisim', label: 'İletişim' },
]

export type Slide = {
  id: number
  title: string
  description: string
  image: string
}

export const slides: Slide[] = [
  {
    id: 1,
    title: '2024 KPSS Yerleştirme Sonuçları Açıklandı',
    description:
      'Merkezi yerleştirme taban puanları güncellendi. Bölümüne göre en güncel atama puanlarını incele.',
    image:
      'https://images.unsplash.com/photo-1523050854058-8df90110c9f1?w=1200&h=500&fit=crop&auto=format',
  },
  {
    id: 2,
    title: 'YKS Tercih Döneminde Doğru Karar',
    description:
      'Üniversite ve bölüm bazında taban puanları ve başarı sıralamalarını karşılaştırarak tercih yap.',
    image:
      'https://images.unsplash.com/photo-1541339907198-e08756dedf3f?w=1200&h=500&fit=crop&auto=format',
  },
  {
    id: 3,
    title: 'DGS ile Lisans Tamamlama Fırsatları',
    description:
      'Önlisans mezunları için geçiş yapılabilecek bölümler ve güncel taban puanları burada.',
    image:
      'https://images.unsplash.com/photo-1592280771190-3e2e4d571952?w=1200&h=500&fit=crop&auto=format',
  },
]

export type Countdown = {
  key: string
  label: string
  target: string // ISO date
}

export const countdowns: Countdown[] = [
  { key: 'yks', label: 'YKS', target: '2026-06-20T10:15:00' },
  { key: 'kpss-orta', label: 'KPSS Ortaöğretim', target: '2026-09-13T10:15:00' },
  { key: 'kpss-onlisans', label: 'KPSS Önlisans', target: '2026-09-06T10:15:00' },
  { key: 'kpss-lisans', label: 'KPSS Lisans', target: '2026-07-19T10:15:00' },
  { key: 'dgs', label: 'DGS', target: '2026-07-05T10:15:00' },
  { key: 'ales', label: 'ALES', target: '2026-05-10T10:15:00' },
]

export type ScoreCard = {
  key: string
  title: string
  category: string
  href: string
}

export const scoreCards: ScoreCard[] = [
  { key: 'kpss-lisans', title: 'KPSS Lisans', category: 'Taban Puanı', href: 'kpss-lisans' },
  { key: 'kpss-onlisans', title: 'KPSS Önlisans', category: 'Taban Puanı', href: 'kpss-onlisans' },
  {
    key: 'kpss-orta',
    title: 'KPSS Ortaöğretim',
    category: 'Taban Puanı',
    href: 'kpss-orta',
  },
  { key: 'yks', title: 'YKS', category: 'Taban Puanı', href: 'yks' },
  { key: 'dgs', title: 'DGS', category: 'Taban Puanı', href: 'dgs' },
]

export type RecentItem = { id: number; title: string; date: string }

export const recentItems: RecentItem[] = [
  { id: 1, title: 'KPSS 2024/2 Atama Taban Puanları', date: '18 Eyl 2024' },
  { id: 2, title: 'Bilgisayar Mühendisliği YKS Sıralamaları', date: '16 Eyl 2024' },
  { id: 3, title: 'Adalet Öğretmenliği Atama Puanları', date: '14 Eyl 2024' },
  { id: 4, title: 'DGS Hemşirelik Geçiş Puanları', date: '11 Eyl 2024' },
]

export type Announcement = { id: number; title: string; date: string }

export const announcements: Announcement[] = [
  { id: 1, title: '2024 KPSS tercih kılavuzu yayımlandı', date: '19 Eyl 2024' },
  { id: 2, title: 'ALES/3 başvuruları başladı', date: '12 Eyl 2024' },
  { id: 3, title: 'YKS ek yerleştirme takvimi açıklandı', date: '05 Eyl 2024' },
]

export type ExamLevel = 'lisans' | 'onlisans' | 'ortaogretim'

export const levelTabs: { key: ExamLevel; label: string }[] = [
  { key: 'lisans', label: 'Lisans' },
  { key: 'onlisans', label: 'Önlisans' },
  { key: 'ortaogretim', label: 'Ortaöğretim' },
]

export type Department = { id: number; name: string; level: ExamLevel }

export const departments: Department[] = [
  { id: 1, name: 'Acil Yardım ve Afet Yönetimi', level: 'lisans' },
  { id: 2, name: 'Adalet Öğretmenliği', level: 'lisans' },
  { id: 3, name: 'Aktüerya', level: 'lisans' },
  { id: 4, name: 'Alman Dili ve Edebiyatı', level: 'lisans' },
  { id: 5, name: 'Bilgisayar Mühendisliği', level: 'lisans' },
  { id: 6, name: 'Elektrik-Elektronik Mühendisliği', level: 'lisans' },
  { id: 7, name: 'Hemşirelik', level: 'lisans' },
  { id: 8, name: 'Hukuk', level: 'lisans' },
  { id: 9, name: 'İşletme', level: 'lisans' },
  { id: 10, name: 'Psikoloji', level: 'lisans' },
  { id: 11, name: 'Adalet', level: 'onlisans' },
  { id: 12, name: 'Bilgisayar Programcılığı', level: 'onlisans' },
  { id: 13, name: 'Büro Yönetimi ve Yönetici Asistanlığı', level: 'onlisans' },
  { id: 14, name: 'İlk ve Acil Yardım', level: 'onlisans' },
  { id: 15, name: 'Muhasebe ve Vergi Uygulamaları', level: 'onlisans' },
  { id: 16, name: 'Tıbbi Dokümantasyon ve Sekreterlik', level: 'onlisans' },
  { id: 17, name: 'Büro Memurluğu', level: 'ortaogretim' },
  { id: 18, name: 'Veri Hazırlama ve Kontrol İşletmeni', level: 'ortaogretim' },
  { id: 19, name: 'Zabıt Katipliği', level: 'ortaogretim' },
  { id: 20, name: 'Hizmetli', level: 'ortaogretim' },
]

export const detailPeriods = ['2024/2', '2024/1', '2023/2', '2023/1']

export type ScoreRow = {
  id: number
  institution: string
  city: string
  title: string
  quota: number
  vacant: number
  minScore: number
  maxScore: number
  qualification: string
}

export const scoreRows: ScoreRow[] = [
  {
    id: 1,
    institution: 'Adalet Bakanlığı',
    city: 'Ankara',
    title: 'Mühendis',
    quota: 12,
    vacant: 0,
    minScore: 80.95434,
    maxScore: 89.21876,
    qualification: '3607 - Bilgisayar Müh.',
  },
  {
    id: 2,
    institution: 'Sağlık Bakanlığı',
    city: 'İstanbul',
    title: 'Mühendis',
    quota: 8,
    vacant: 0,
    minScore: 80.99622,
    maxScore: 81.21216,
    qualification: '3607 - Bilgisayar Müh.',
  },
  {
    id: 3,
    institution: 'Milli Eğitim Bakanlığı',
    city: 'İzmir',
    title: 'Mühendis',
    quota: 5,
    vacant: 1,
    minScore: 78.4512,
    maxScore: 84.7719,
    qualification: '3607 - Bilgisayar Müh.',
  },
  {
    id: 4,
    institution: 'Çevre, Şehircilik ve İklim Değişikliği Bakanlığı',
    city: 'Bursa',
    title: 'Mühendis',
    quota: 6,
    vacant: 0,
    minScore: 79.1033,
    maxScore: 83.4102,
    qualification: '3607 - Bilgisayar Müh.',
  },
  {
    id: 5,
    institution: 'Hazine ve Maliye Bakanlığı',
    city: 'Ankara',
    title: 'Mühendis',
    quota: 10,
    vacant: 0,
    minScore: 82.331,
    maxScore: 90.0021,
    qualification: '3607 - Bilgisayar Müh.',
  },
  {
    id: 6,
    institution: 'Ulaştırma ve Altyapı Bakanlığı',
    city: 'Antalya',
    title: 'Mühendis',
    quota: 4,
    vacant: 2,
    minScore: 76.8801,
    maxScore: 82.1109,
    qualification: '3607 - Bilgisayar Müh.',
  },
  {
    id: 7,
    institution: 'Gençlik ve Spor Bakanlığı',
    city: 'Konya',
    title: 'Mühendis',
    quota: 3,
    vacant: 0,
    minScore: 77.5522,
    maxScore: 80.9931,
    qualification: '3607 - Bilgisayar Müh.',
  },
  {
    id: 8,
    institution: 'Tarım ve Orman Bakanlığı',
    city: 'Samsun',
    title: 'Mühendis',
    quota: 7,
    vacant: 1,
    minScore: 75.2201,
    maxScore: 81.4407,
    qualification: '3607 - Bilgisayar Müh.',
  },
  {
    id: 9,
    institution: 'İçişleri Bakanlığı',
    city: 'Ankara',
    title: 'Mühendis',
    quota: 9,
    vacant: 0,
    minScore: 83.7712,
    maxScore: 91.2298,
    qualification: '3607 - Bilgisayar Müh.',
  },
  {
    id: 10,
    institution: 'Sanayi ve Teknoloji Bakanlığı',
    city: 'Kocaeli',
    title: 'Mühendis',
    quota: 5,
    vacant: 0,
    minScore: 79.9012,
    maxScore: 85.5533,
    qualification: '3607 - Bilgisayar Müh.',
  },
]

// --- İlanlar (kamu personel alım ilanları) ---

export type JobCategory = { key: string; label: string; children?: JobCategory[] }

export const jobCategories: JobCategory[] = [
  { key: 'all', label: 'Tüm İlanlar' },
  {
    key: 'memur',
    label: 'Memur',
    children: [
      { key: 'memur-a', label: 'A Grubu Memur (Kariyer Meslek)' },
      { key: 'memur-b', label: 'B Grubu Memur' },
    ],
  },
  {
    key: 'sozlesmeli',
    label: 'Sözleşmeli Personel',
    children: [
      { key: 'sozlesmeli-kariyer', label: 'Kariyer Sözleşmeli Personel' },
      { key: 'sozlesmeli-4b', label: '4/B Sözleşmeli Personel' },
      { key: 'sozlesmeli-kit', label: 'KİT Sözleşmeli Personel' },
      { key: 'sozlesmeli-kurumsal', label: 'Kurumsal Sözleşmeli Personel' },
    ],
  },
  {
    key: 'akademik',
    label: 'Akademik Personel',
    children: [
      { key: 'akademik-uye', label: 'Öğretim Üyesi' },
      { key: 'akademik-gorevli', label: 'Öğretim Görevlisi' },
      { key: 'akademik-arastirma', label: 'Araştırma Görevlisi' },
    ],
  },
  {
    key: 'isci',
    label: 'İşçi',
    children: [
      { key: 'isci-kariyer', label: 'Kariyer İşçi' },
      { key: 'isci-surekli', label: 'Sürekli İşçi' },
      { key: 'isci-gecici', label: 'Geçici İşçi' },
      { key: 'isci-engelli', label: 'Engelli İşçi' },
      { key: 'isci-eski-hukumlu', label: 'Eski Hükümlü İşçi' },
    ],
  },
  { key: 'askeri', label: 'Askeri Personel' },
  { key: 'yargi', label: 'Yargı Mensubu (Hakim - Savcı)' },
]

export type JobListing = {
  id: number
  institution: string
  summary: string
  categoryKey: string
  categoryLabel: string
  publishedAt: string // ISO date, timeline gruplaması için
  startDate: string
  endDate: string
}

export const jobListings: JobListing[] = [
  {
    id: 1,
    institution: 'Adana Alparslan Türkeş Bilim ve Teknoloji Üniversitesi',
    summary: '25 Öğretim Elemanı Alacak',
    categoryKey: 'akademik-uye',
    categoryLabel: 'Öğretim Üyesi',
    publishedAt: '2026-09-20',
    startDate: '20 Eylül',
    endDate: '4 Ekim',
  },
  {
    id: 2,
    institution: 'Munzur Üniversitesi',
    summary: '1 Öğretim Üyesi İptal İlanı',
    categoryKey: 'akademik-uye',
    categoryLabel: 'Öğretim Üyesi',
    publishedAt: '2026-09-19',
    startDate: '29 Eylül',
    endDate: '29 Eylül',
  },
  {
    id: 3,
    institution: 'İstanbul Teknik Üniversitesi',
    summary: '44 Öğretim Üyesi Alacak',
    categoryKey: 'akademik-uye',
    categoryLabel: 'Öğretim Üyesi',
    publishedAt: '2026-09-18',
    startDate: '18 Eylül',
    endDate: '2 Ekim',
  },
  {
    id: 4,
    institution: 'Karabük Üniversitesi',
    summary: 'Öğretim Elemanı Düzeltme İlanı',
    categoryKey: 'akademik-gorevli',
    categoryLabel: 'Öğretim Görevlisi',
    publishedAt: '2026-09-18',
    startDate: '18 Eylül',
    endDate: '2 Ekim',
  },
  {
    id: 5,
    institution: 'İzmir Demokrasi Üniversitesi',
    summary: '20 Öğretim Üyesi Alacak',
    categoryKey: 'akademik-uye',
    categoryLabel: 'Öğretim Üyesi',
    publishedAt: '2026-09-18',
    startDate: '18 Eylül',
    endDate: '2 Ekim',
  },
  {
    id: 6,
    institution: 'Niğde Ömer Halisdemir Üniversitesi',
    summary: '120 Öğretim Üyesi Alacak',
    categoryKey: 'akademik-uye',
    categoryLabel: 'Öğretim Üyesi',
    publishedAt: '2026-09-18',
    startDate: '18 Eylül',
    endDate: '5 Ekim',
  },
  {
    id: 7,
    institution: 'Gençlik ve Spor Bakanlığı',
    summary: '9 Bilişim Personeli Alacak',
    categoryKey: 'sozlesmeli-4b',
    categoryLabel: '4/B Sözleşmeli Personel',
    publishedAt: '2026-09-18',
    startDate: '21 Eylül',
    endDate: '25 Eylül',
  },
  {
    id: 8,
    institution: 'İletişim Başkanlığı',
    summary: '15 Uzman Yardımcısı Alacak',
    categoryKey: 'memur-a',
    categoryLabel: 'A Grubu Memur (Kariyer Meslek)',
    publishedAt: '2026-09-18',
    startDate: '5 Ekim',
    endDate: '20 Ekim',
  },
  {
    id: 9,
    institution: 'Sivas Bilim ve Teknoloji Üniversitesi',
    summary: '9 Sözleşmeli Personel Alacak',
    categoryKey: 'sozlesmeli-kurumsal',
    categoryLabel: 'Kurumsal Sözleşmeli Personel',
    publishedAt: '2026-09-18',
    startDate: '18 Eylül',
    endDate: '2 Ekim',
  },
  {
    id: 10,
    institution: 'Tokat Gaziosmanpaşa Üniversitesi',
    summary: '30 Araştırma Görevlisi Alacak',
    categoryKey: 'akademik-arastirma',
    categoryLabel: 'Araştırma Görevlisi',
    publishedAt: '2026-09-17',
    startDate: '17 Eylül',
    endDate: '1 Ekim',
  },
  {
    id: 11,
    institution: 'Sağlık Bakanlığı',
    summary: '3.500 Sürekli İşçi Alacak',
    categoryKey: 'isci-surekli',
    categoryLabel: 'Sürekli İşçi',
    publishedAt: '2026-09-17',
    startDate: '17 Eylül',
    endDate: '30 Eylül',
  },
  {
    id: 12,
    institution: 'Adalet Bakanlığı',
    summary: '250 Zabıt Kâtibi (B Grubu) Alacak',
    categoryKey: 'memur-b',
    categoryLabel: 'B Grubu Memur',
    publishedAt: '2026-09-16',
    startDate: '16 Eylül',
    endDate: '29 Eylül',
  },
  {
    id: 13,
    institution: 'Jandarma Genel Komutanlığı',
    summary: '1.200 Uzman Erbaş Alacak',
    categoryKey: 'askeri',
    categoryLabel: 'Askeri Personel',
    publishedAt: '2026-09-16',
    startDate: '16 Eylül',
    endDate: '10 Ekim',
  },
  {
    id: 14,
    institution: 'Hâkimler ve Savcılar Kurulu',
    summary: '150 Adli Yargı Hâkim Adayı Alacak',
    categoryKey: 'yargi',
    categoryLabel: 'Yargı Mensubu (Hakim - Savcı)',
    publishedAt: '2026-09-15',
    startDate: '15 Eylül',
    endDate: '3 Ekim',
  },
]

export type Comment = {
  id: number
  author: string
  time: string
  body: string
  likes: number
  replies: Comment[]
}

export const comments: Comment[] = [
  {
    id: 1,
    author: 'Ahmet Y.',
    time: '2 gün önce',
    body: 'KPSS lisans için bu puanlar yeterli mi? Bilgisayar mühendisliği düşünüyorum.',
    likes: 3,
    replies: [
      {
        id: 2,
        author: 'Ayşe K.',
        time: '1 gün önce',
        body: 'Geçen dönem bu puanla atananlar oldu, ama kadro sayısına da bakmak lazım.',
        likes: 1,
        replies: [
          {
            id: 3,
            author: 'Ahmet Y.',
            time: '5 saat önce',
            body: 'Teşekkürler, kadro sayısına bakayım.',
            likes: 0,
            replies: [],
          },
        ],
      },
      {
        id: 4,
        author: 'Mehmet T.',
        time: '20 saat önce',
        body: 'Kurum tercihine göre değişir. Ankara dışını da işaretlersen şansın artar.',
        likes: 2,
        replies: [],
      },
    ],
  },
  {
    id: 5,
    author: 'Zeynep D.',
    time: '3 gün önce',
    body: 'Sağlık Bakanlığı kadrolarında taban puan biraz daha düşük görünüyor.',
    likes: 4,
    replies: [],
  },
]
