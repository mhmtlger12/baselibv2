import type { ReactNode } from 'react'
import { api } from '../../api/client'
import { useApiData } from '../../api/useApiData'
import { Panel, SectionHead } from '../ui'
import { IconUsers, IconRoles, IconTree, IconActivity } from '../icons'

const colors = ['#0f9f8f', '#0a192f', '#f59e0b', '#6366f1', '#10b981', '#ef4444']

export default function Dashboard() {
  const stats = useApiData(api.dashboard)
  const logs = useApiData(api.auditLogs)
  if (stats.loading) return <Panel className="p-6 text-muted-foreground">Gösterge paneli yükleniyor…</Panel>
  if (stats.error) return <Panel className="p-6 text-rose-600">{stats.error}</Panel>
  const data = stats.data!
  const cards = [
    { label: 'Toplam Kullanıcı', value: data.totalUsers, hint: `${data.activeUsers} aktif`, accent: 'from-navy-500 to-navy-700', icon: <IconUsers className="h-5 w-5" /> },
    { label: 'Tanımlı Rol', value: data.totalRoles, hint: 'RBAC yetkilendirme', accent: 'from-teal-500 to-teal-700', icon: <IconRoles className="h-5 w-5" /> },
    { label: 'Departman', value: data.totalDepartments, hint: 'Organizasyon ağacı', accent: 'from-sky-500 to-navy-500', icon: <IconTree className="h-5 w-5" /> },
  ]
  return <div>
    <SectionHead title="Sistem Gösterge Paneli" description="Kullanıcı, rol ve organizasyon istatistiklerinin API’den gelen güncel özeti." />
    <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">{cards.map((card) => <StatCard key={card.label} {...card} />)}</div>
    <div className="mt-4 grid grid-cols-1 gap-4 xl:grid-cols-3">
      <Panel className="p-6"><h3 className="text-sm font-bold text-navy-900">Rol Dağılımı</h3><p className="text-xs text-muted-foreground">Kullanıcıların rollere göre dağılımı</p><div className="mt-5 flex items-center gap-6"><Donut data={data.roleDistributions.map((item, index) => ({ name: item.roleName, value: item.userCount, color: colors[index % colors.length] }))} /><ul className="min-w-0 flex-1 space-y-2.5">{data.roleDistributions.map((item, index) => <li key={item.roleName} className="flex gap-2 text-sm"><span className="mt-1.5 h-2.5 w-2.5 shrink-0 rounded-full" style={{ background: colors[index % colors.length] }} /><span className="truncate text-navy-700">{item.roleName}</span><span className="ml-auto font-semibold text-navy-900">{item.userCount}</span></li>)}</ul></div></Panel>
      <Panel className="p-6 xl:col-span-2"><div className="flex items-center gap-2"><IconActivity className="h-5 w-5 text-teal-500" /><div><h3 className="text-sm font-bold text-navy-900">Son Sistem Hareketleri</h3><p className="text-xs text-muted-foreground">Denetim kaydından alınan son işlemler</p></div></div>{logs.loading ? <p className="mt-5 text-sm text-muted-foreground">Yükleniyor…</p> : logs.error ? <p className="mt-5 text-sm text-rose-600">{logs.error}</p> : <ul className="mt-4 divide-y divide-navy-50">{(logs.data ?? []).slice(0, 5).map((log) => <li key={log.id} className="flex items-center gap-3 py-2.5"><span className="h-2 w-2 shrink-0 rounded-full bg-teal-500" /><span className="text-sm text-navy-800">{log.details || `${log.controller} / ${log.action}`}</span><span className="ml-auto whitespace-nowrap font-mono text-xs text-navy-400">{formatDate(log.createdDate)}</span></li>)}</ul>}</Panel>
    </div>
  </div>
}

function StatCard({ label, value, hint, accent, icon }: { label: string; value: number; hint: string; accent: string; icon: ReactNode }) {
  return <Panel className="overflow-hidden p-5"><div className="flex items-start justify-between"><div className={`flex h-11 w-11 items-center justify-center rounded-xl bg-gradient-to-br text-white shadow-sm ${accent}`}>{icon}</div><span className="text-3xl font-bold tracking-tight text-navy-900">{value}</span></div><p className="mt-4 text-sm font-semibold text-navy-800">{label}</p><p className="text-xs text-muted-foreground">{hint}</p></Panel>
}

function Donut({ data }: { data: { name: string; value: number; color: string }[] }) {
  const total = data.reduce((sum, item) => sum + item.value, 0)
  if (!total) return <div className="flex h-[130px] w-[130px] items-center justify-center rounded-full border-[18px] border-navy-50 text-sm text-muted-foreground">Veri yok</div>
  const radius = 52; const circumference = 2 * Math.PI * radius; let offset = 0
  return <svg width="130" height="130" viewBox="0 0 130 130" className="shrink-0 -rotate-90"><circle cx="65" cy="65" r={radius} fill="none" stroke="#eef3fb" strokeWidth="18" />{data.map((item) => { const length = (item.value / total) * circumference; const circle = <circle key={item.name} cx="65" cy="65" r={radius} fill="none" stroke={item.color} strokeWidth="18" strokeDasharray={`${length} ${circumference - length}`} strokeDashoffset={-offset} />; offset += length; return circle })}<text x="65" y="65" textAnchor="middle" dominantBaseline="central" transform="rotate(90 65 65)" style={{ fontSize: 22, fontWeight: 700, fill: '#101d3a' }}>{total}</text></svg>
}

function formatDate(value: string) { return new Intl.DateTimeFormat('tr-TR', { dateStyle: 'short', timeStyle: 'short' }).format(new Date(value)) }
