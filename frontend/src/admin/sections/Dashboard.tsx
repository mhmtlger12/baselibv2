import { Panel, SectionHead } from '../ui'
import { IconUsers, IconRoles, IconTree, IconActivity, IconKey } from '../icons'
import {
  adminUsers,
  adminRoles,
  adminDepartments,
  adminPermissions,
  roleDistribution,
  visitTrend,
  auditLogs,
} from '../adminData'
import type { ReactNode } from 'react'

const stats = [
  {
    label: 'Toplam Kullanıcı',
    value: adminUsers.length,
    hint: `${adminUsers.filter((u) => u.active).length} aktif`,
    accent: 'from-navy-500 to-navy-700',
    icon: <IconUsers className="h-5 w-5" />,
  },
  {
    label: 'Tanımlı Rol',
    value: adminRoles.length,
    hint: 'RBAC yetkilendirme',
    accent: 'from-teal-500 to-teal-700',
    icon: <IconRoles className="h-5 w-5" />,
  },
  {
    label: 'İzin Sayısı',
    value: adminPermissions.length,
    hint: 'Controller bazlı',
    accent: 'from-amber-400 to-amber-500',
    icon: <IconKey className="h-5 w-5" />,
  },
  {
    label: 'Departman',
    value: adminDepartments.length,
    hint: 'Organizasyon ağacı',
    accent: 'from-sky-500 to-navy-500',
    icon: <IconTree className="h-5 w-5" />,
  },
]

export default function Dashboard() {
  return (
    <div>
      <SectionHead
        title="Sistem Gösterge Paneli"
        description="Kullanıcı, rol ve sistem istatistiklerinin güncel özeti. Tüm veriler API üzerinden beslenecektir."
      />

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-4">
        {stats.map((s) => (
          <Panel key={s.label} className="overflow-hidden p-5">
            <div className="flex items-start justify-between">
              <div
                className={`flex h-11 w-11 items-center justify-center rounded-xl bg-gradient-to-br text-white shadow-sm ${s.accent}`}
              >
                {s.icon}
              </div>
              <span className="text-3xl font-bold tracking-tight text-navy-900">{s.value}</span>
            </div>
            <p className="mt-4 text-sm font-semibold text-navy-800">{s.label}</p>
            <p className="text-xs text-muted-foreground">{s.hint}</p>
          </Panel>
        ))}
      </div>

      <div className="mt-4 grid grid-cols-1 gap-4 xl:grid-cols-3">
        {/* Rol dağılımı donut */}
        <Panel className="p-6 xl:col-span-1">
          <h3 className="text-sm font-bold text-navy-900">Rol Dağılımı</h3>
          <p className="text-xs text-muted-foreground">Kullanıcıların rollere göre dağılımı</p>
          <div className="mt-5 flex items-center gap-6">
            <Donut data={roleDistribution} />
            <ul className="space-y-2.5">
              {roleDistribution.map((r) => (
                <li key={r.name} className="flex items-center gap-2 text-sm">
                  <span className="h-2.5 w-2.5 rounded-full" style={{ background: r.color }} />
                  <span className="text-navy-700">{r.name}</span>
                  <span className="ml-auto font-semibold text-navy-900">{r.value}</span>
                </li>
              ))}
            </ul>
          </div>
        </Panel>

        {/* Ziyaret trendi */}
        <Panel className="p-6 xl:col-span-2">
          <div className="flex items-center justify-between">
            <div>
              <h3 className="text-sm font-bold text-navy-900">Haftalık Ziyaret Trendi</h3>
              <p className="text-xs text-muted-foreground">Son 7 gün · sitedeki ziyaretçi hareketi</p>
            </div>
            <IconActivity className="h-5 w-5 text-teal-500" />
          </div>
          <TrendBars data={visitTrend} />
        </Panel>
      </div>

      <div className="mt-4 grid grid-cols-1 gap-4 xl:grid-cols-3">
        {/* Hızlı işlemler */}
        <Panel className="p-6">
          <h3 className="text-sm font-bold text-navy-900">Hızlı İşlemler</h3>
          <ul className="mt-4 space-y-1">
            <QuickAction
              icon={<IconUsers className="h-5 w-5" />}
              title="Kullanıcı Yönetimi"
              desc="Sisteme yeni kullanıcı ekleyin veya düzenleyin."
            />
            <QuickAction
              icon={<IconRoles className="h-5 w-5" />}
              title="Rol Yetkilendirmesi"
              desc="Rollere bağlı modül erişim izinlerini ayarlayın."
            />
            <QuickAction
              icon={<IconTree className="h-5 w-5" />}
              title="Departman Organizasyonu"
              desc="Alt/üst departman hiyerarşisini güncelleyin."
            />
          </ul>
        </Panel>

        {/* Son sistem hareketleri */}
        <Panel className="p-6 xl:col-span-2">
          <h3 className="text-sm font-bold text-navy-900">Son Sistem Hareketleri</h3>
          <ul className="mt-4 divide-y divide-navy-50">
            {auditLogs.slice(0, 5).map((log) => (
              <li key={log.id} className="flex items-center gap-3 py-2.5">
                <span
                  className={`h-2 w-2 shrink-0 rounded-full ${
                    log.level === 'danger'
                      ? 'bg-rose-500'
                      : log.level === 'warning'
                        ? 'bg-amber-500'
                        : 'bg-teal-500'
                  }`}
                />
                <span className="text-sm text-navy-800">{log.detail}</span>
                <span className="ml-auto whitespace-nowrap font-mono text-xs text-navy-400">
                  {log.date.slice(5)}
                </span>
              </li>
            ))}
          </ul>
        </Panel>
      </div>
    </div>
  )
}

function QuickAction({ icon, title, desc }: { icon: ReactNode; title: string; desc: string }) {
  return (
    <li>
      <button className="flex w-full items-start gap-3 rounded-xl px-3 py-3 text-left transition-colors hover:bg-navy-50">
        <span className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-teal-50 text-teal-600">
          {icon}
        </span>
        <span>
          <span className="block text-sm font-semibold text-navy-800">{title}</span>
          <span className="block text-xs text-muted-foreground">{desc}</span>
        </span>
      </button>
    </li>
  )
}

/** SVG donut grafik. */
function Donut({ data }: { data: { name: string; value: number; color: string }[] }) {
  const total = data.reduce((sum, d) => sum + d.value, 0)
  const radius = 52
  const circ = 2 * Math.PI * radius
  let offset = 0

  return (
    <svg width="130" height="130" viewBox="0 0 130 130" className="shrink-0 -rotate-90">
      <circle cx="65" cy="65" r={radius} fill="none" stroke="#eef3fb" strokeWidth="18" />
      {data.map((d) => {
        const len = (d.value / total) * circ
        const seg = (
          <circle
            key={d.name}
            cx="65"
            cy="65"
            r={radius}
            fill="none"
            stroke={d.color}
            strokeWidth="18"
            strokeDasharray={`${len} ${circ - len}`}
            strokeDashoffset={-offset}
            strokeLinecap="butt"
          />
        )
        offset += len
        return seg
      })}
      <text
        x="65"
        y="65"
        textAnchor="middle"
        dominantBaseline="central"
        className="rotate-90"
        transform="rotate(90 65 65)"
        style={{ fontSize: 22, fontWeight: 700, fill: '#101d3a' }}
      >
        {total}
      </text>
    </svg>
  )
}

/** Basit bar grafik. */
function TrendBars({ data }: { data: { day: string; value: number }[] }) {
  const max = Math.max(...data.map((d) => d.value))
  return (
    <div className="mt-6 flex h-44 items-end gap-3">
      {data.map((d) => (
        <div key={d.day} className="flex flex-1 flex-col items-center gap-2">
          <div className="flex w-full flex-1 items-end">
            <div
              className="w-full rounded-t-lg bg-gradient-to-t from-navy-200 to-teal-400 transition-all hover:from-navy-300 hover:to-teal-500"
              style={{ height: `${(d.value / max) * 100}%` }}
              title={`${d.value.toLocaleString('tr-TR')} ziyaret`}
            />
          </div>
          <span className="text-xs text-muted-foreground">{d.day}</span>
        </div>
      ))}
    </div>
  )
}
