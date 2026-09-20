// Örnek panel verisi. Gerçek veriler API'den gelecek; burası yalnızca arayüzü
// beslemek için tutulan statik örnek datadır.

export type AdminUser = {
  id: number
  firstName: string
  lastName: string
  username: string
  email: string
  phone: string
  department: string
  roles: string[]
  active: boolean
  lastLogin: string
}

export const adminUsers: AdminUser[] = [
  {
    id: 1,
    firstName: 'Admin',
    lastName: 'User',
    username: 'admin',
    email: 'admin@puannokta.com',
    phone: '+90 532 000 00 00',
    department: 'Yönetim',
    roles: ['Admin'],
    active: true,
    lastLogin: '2026-09-20 17:12',
  },
  {
    id: 2,
    firstName: 'Elif',
    lastName: 'Yıldız',
    username: 'elif.editor',
    email: 'elif@puannokta.com',
    phone: '+90 533 111 22 33',
    department: 'İçerik',
    roles: ['Editör'],
    active: true,
    lastLogin: '2026-09-20 14:48',
  },
  {
    id: 3,
    firstName: 'Mert',
    lastName: 'Kaya',
    username: 'mert.moderator',
    email: 'mert@puannokta.com',
    phone: '+90 542 444 55 66',
    department: 'Moderasyon',
    roles: ['Moderatör'],
    active: true,
    lastLogin: '2026-09-19 22:03',
  },
  {
    id: 4,
    firstName: 'Zeynep',
    lastName: 'Demir',
    username: 'zeynep.analiz',
    email: 'zeynep@puannokta.com',
    phone: '+90 505 777 88 99',
    department: 'Veri',
    roles: ['Analist', 'Editör'],
    active: false,
    lastLogin: '2026-09-11 09:20',
  },
]

// Oturum açan kullanıcı. Birden fazla rolü olan kullanıcılar profil ekranından
// istedikleri rol ile sisteme giriş yapabilir.
export type CurrentUser = {
  firstName: string
  lastName: string
  username: string
  email: string
  department: string
  registeredAt: string
  roles: string[]
}

export const currentUser: CurrentUser = {
  firstName: 'Admin',
  lastName: 'User',
  username: 'admin',
  email: 'admin@puannokta.com',
  department: 'Yönetim',
  registeredAt: '01.01.2025',
  roles: ['Admin', 'Editör', 'Müşteri'],
}

export type AdminRole = {
  id: number
  name: string
  description: string
  active: boolean
  permissionCount: number
  userCount: number
}

export const adminRoles: AdminRole[] = [
  { id: 1, name: 'Admin', description: 'Tüm modüllere tam yetki', active: true, permissionCount: 30, userCount: 1 },
  { id: 2, name: 'Editör', description: 'İçerik ekleme ve düzenleme', active: true, permissionCount: 14, userCount: 2 },
  { id: 3, name: 'Moderatör', description: 'Yorum ve kullanıcı moderasyonu', active: true, permissionCount: 9, userCount: 1 },
  { id: 4, name: 'Analist', description: 'Salt okunur raporlama erişimi', active: false, permissionCount: 5, userCount: 1 },
]

export type CrudType = 'View' | 'Add' | 'Update' | 'Delete' | 'Option' | 'Preview'

export type AdminPermission = {
  id: number
  name: string
  code: string
  controller: string
  action: string
  description: string
  crudType: CrudType
  active: boolean
}

export const adminPermissions: AdminPermission[] = [
  { id: 1, name: 'Hareket Listele', code: 'AuditLogs_Read', controller: 'AuditLogs', action: 'List', description: 'Sistem hareketlerini (logları) görüntüleme', crudType: 'View', active: true },
  { id: 2, name: 'Dashboard Görüntüle', code: 'Dashboard_Read', controller: 'Dashboard', action: 'GetStats', description: 'Dashboard istatistiklerini görüntüleme', crudType: 'View', active: true },
  { id: 3, name: 'Departman Listele', code: 'Departments_Read', controller: 'Departments', action: 'List', description: 'Departman listeleme ve görüntüleme', crudType: 'View', active: true },
  { id: 4, name: 'Departman Oluştur', code: 'Departments_Create', controller: 'Departments', action: 'Add', description: 'Departman oluşturma', crudType: 'Add', active: true },
  { id: 5, name: 'Departman Güncelle', code: 'Departments_Update', controller: 'Departments', action: 'Update', description: 'Departman güncelleme', crudType: 'Update', active: true },
  { id: 6, name: 'Departman Seçenekleri', code: 'Departments_SelectOption', controller: 'Departments', action: 'SelectOption', description: 'Departman seçim listelerini görüntüleme', crudType: 'Option', active: true },
  { id: 7, name: 'Departman Sil', code: 'Departments_Delete', controller: 'Departments', action: 'Delete', description: 'Departman silme', crudType: 'Delete', active: true },
  { id: 8, name: 'Menü Listele', code: 'Menus_Read', controller: 'Menus', action: 'List', description: 'Menü listeleme ve görüntüleme', crudType: 'View', active: true },
  { id: 9, name: 'Menü Oluştur', code: 'Menus_Create', controller: 'Menus', action: 'Add', description: 'Menü oluşturma', crudType: 'Add', active: true },
  { id: 10, name: 'Menü Güncelle', code: 'Menus_Update', controller: 'Menus', action: 'Update', description: 'Menü güncelleme', crudType: 'Update', active: true },
  { id: 11, name: 'Menü Sil', code: 'Menus_Delete', controller: 'Menus', action: 'Delete', description: 'Menü silme', crudType: 'Delete', active: true },
  { id: 12, name: 'Kullanıcı Listele', code: 'Users_Read', controller: 'Users', action: 'List', description: 'Kullanıcı listeleme ve görüntüleme', crudType: 'View', active: true },
  { id: 13, name: 'Kullanıcı Oluştur', code: 'Users_Create', controller: 'Users', action: 'Add', description: 'Kullanıcı oluşturma', crudType: 'Add', active: true },
  { id: 14, name: 'Kullanıcı Güncelle', code: 'Users_Update', controller: 'Users', action: 'Update', description: 'Kullanıcı güncelleme', crudType: 'Update', active: true },
  { id: 15, name: 'Kullanıcı Sil', code: 'Users_Delete', controller: 'Users', action: 'Delete', description: 'Kullanıcı silme', crudType: 'Delete', active: true },
  { id: 16, name: 'Rol Listele', code: 'Roles_Read', controller: 'Roles', action: 'List', description: 'Rol listeleme ve görüntüleme', crudType: 'View', active: true },
  { id: 17, name: 'Rol Oluştur', code: 'Roles_Create', controller: 'Roles', action: 'Add', description: 'Rol oluşturma', crudType: 'Add', active: true },
  { id: 18, name: 'Ayar Güncelle', code: 'Settings_Update', controller: 'Settings', action: 'Update', description: 'Sistem ayarlarını güncelleme', crudType: 'Update', active: true },
]

export type AdminDepartment = {
  id: number
  name: string
  code: string
  active: boolean
  parentId: number | null
}

export const adminDepartments: AdminDepartment[] = [
  { id: 1, name: 'Yönetim', code: 'YT', active: true, parentId: null },
  { id: 2, name: 'Bilgi Teknolojileri', code: 'BT', active: true, parentId: 1 },
  { id: 3, name: 'İnsan Kaynakları', code: 'İK', active: true, parentId: 1 },
  { id: 4, name: 'İçerik', code: 'IC', active: true, parentId: 2 },
  { id: 5, name: 'Moderasyon', code: 'MOD', active: true, parentId: 2 },
  { id: 6, name: 'Veri', code: 'VR', active: false, parentId: 2 },
]

export type AdminMenu = {
  id: number
  name: string
  url: string
  icon: string
  order: number
  permission: string
  active: boolean
}

export const adminMenus: AdminMenu[] = [
  { id: 1, name: 'Dashboard', url: '/Admin/Dashboard', icon: 'bi-speedometer2', order: 0, permission: 'Dashboard_Read', active: true },
  { id: 2, name: 'Kullanıcılar', url: '/Admin/Users', icon: 'bi-people', order: 1, permission: 'Users_Read', active: true },
  { id: 3, name: 'Roller', url: '/Admin/Roles', icon: 'bi-shield-check', order: 2, permission: 'Roles_Read', active: true },
  { id: 4, name: 'İzinler', url: '/Admin/Permissions', icon: 'bi-key', order: 3, permission: 'Permissions_Read', active: true },
  { id: 5, name: 'Departmanlar', url: '/Admin/Departments', icon: 'bi-diagram-3', order: 4, permission: 'Departments_Read', active: true },
  { id: 6, name: 'Menüler', url: '/Admin/Menus', icon: 'bi-list-nested', order: 5, permission: 'Menus_Read', active: true },
  { id: 7, name: 'Sistem Ayarları', url: '/Admin/Settings', icon: 'bi-gear', order: 6, permission: 'Settings_Update', active: true },
  { id: 8, name: 'Sistem Hareketleri', url: '/Admin/AuditLogs', icon: 'bi-activity', order: 7, permission: 'AuditLogs_Read', active: true },
  { id: 9, name: 'Çöp Kutusu', url: '/Admin/RecycleBin', icon: 'bi-trash', order: 8, permission: 'RecycleBin_Read', active: true },
]

export type AuditLog = {
  id: number
  user: string
  action: string
  entity: string
  detail: string
  ip: string
  date: string
  level: 'info' | 'warning' | 'danger'
}

export const auditLogs: AuditLog[] = [
  { id: 1, user: 'admin', action: 'Giriş', entity: 'Auth', detail: 'Panele başarılı giriş yapıldı', ip: '88.230.14.7', date: '2026-09-20 17:12', level: 'info' },
  { id: 2, user: 'elif.editor', action: 'Güncelleme', entity: 'ScoreCard', detail: 'YKS taban puanı güncellendi (Bilgisayar Müh.)', ip: '78.180.44.2', date: '2026-09-20 15:40', level: 'info' },
  { id: 3, user: 'mert.moderator', action: 'Silme', entity: 'Comment', detail: 'Spam yorum kaldırıldı #4821', ip: '95.70.128.9', date: '2026-09-20 12:05', level: 'warning' },
  { id: 4, user: 'admin', action: 'Yetki', entity: 'Role', detail: 'Editör rolüne 2 izin eklendi', ip: '88.230.14.7', date: '2026-09-19 18:22', level: 'info' },
  { id: 5, user: 'zeynep.analiz', action: 'Hatalı Giriş', entity: 'Auth', detail: '3 başarısız giriş denemesi', ip: '176.234.5.11', date: '2026-09-19 09:18', level: 'danger' },
  { id: 6, user: 'admin', action: 'Oluşturma', entity: 'User', detail: 'Yeni kullanıcı eklendi: mert.moderator', ip: '88.230.14.7', date: '2026-09-18 11:47', level: 'info' },
]

export type RecycleItem = {
  id: number
  entity: string
  name: string
  deletedBy: string
  deletedAt: string
}

export const recycleItems: RecycleItem[] = [
  { id: 1, entity: 'Kullanıcı', name: 'test.kullanici', deletedBy: 'admin', deletedAt: '2026-09-17 10:12' },
  { id: 2, entity: 'Yorum', name: 'Spam yorum #4390', deletedBy: 'mert.moderator', deletedAt: '2026-09-16 21:34' },
  { id: 3, entity: 'Departman', name: 'Arşiv', deletedBy: 'admin', deletedAt: '2026-09-14 08:50' },
  { id: 4, entity: 'Menü', name: 'Eski Kampanya', deletedBy: 'admin', deletedAt: '2026-09-10 16:05' },
]

// Rol dağılımı (dashboard donut)
export const roleDistribution = [
  { name: 'Admin', value: 1, color: '#1c3568' },
  { name: 'Editör', value: 2, color: '#159a88' },
  { name: 'Moderatör', value: 1, color: '#34b7a4' },
  { name: 'Analist', value: 1, color: '#f5b544' },
]

// Son 7 günlük ziyaret trendi (dashboard mini-grafik)
export const visitTrend = [
  { day: 'Pzt', value: 4200 },
  { day: 'Sal', value: 5100 },
  { day: 'Çar', value: 4800 },
  { day: 'Per', value: 6300 },
  { day: 'Cum', value: 7200 },
  { day: 'Cmt', value: 5600 },
  { day: 'Paz', value: 4900 },
]
