import { useMemo, useState } from 'react'
import { Panel, SectionHead, Badge, SearchInput } from '../ui'
import { auditLogs, type AuditLog } from '../adminData'

const levelTone: Record<AuditLog['level'], 'active' | 'warning' | 'danger'> = {
  info: 'active',
  warning: 'warning',
  danger: 'danger',
}
const levelLabel: Record<AuditLog['level'], string> = {
  info: 'Bilgi',
  warning: 'Uyarı',
  danger: 'Kritik',
}

export default function AuditLogs() {
  const [query, setQuery] = useState('')
  const [level, setLevel] = useState<'all' | AuditLog['level']>('all')

  const filtered = useMemo(() => {
    const q = query.trim().toLowerCase()
    return auditLogs.filter((l) => {
      const matchQ = !q || [l.user, l.action, l.entity, l.detail, l.ip].join(' ').toLowerCase().includes(q)
      const matchL = level === 'all' || l.level === level
      return matchQ && matchL
    })
  }, [query, level])

  const tabs: { key: 'all' | AuditLog['level']; label: string }[] = [
    { key: 'all', label: 'Tümü' },
    { key: 'info', label: 'Bilgi' },
    { key: 'warning', label: 'Uyarı' },
    { key: 'danger', label: 'Kritik' },
  ]

  return (
    <div>
      <SectionHead
        title="Sistem Hareketleri"
        description="Kullanıcı ve sistem işlemlerinin (audit log) kayıtları. Güvenlik ve denetim amaçlıdır."
      />

      <Panel className="overflow-hidden">
        <div className="flex flex-wrap items-center justify-between gap-3 border-b border-navy-50 px-5 py-4">
          <div className="flex gap-1 rounded-xl bg-navy-50 p-1">
            {tabs.map((t) => (
              <button
                key={t.key}
                onClick={() => setLevel(t.key)}
                className={`rounded-lg px-3 py-1.5 text-sm font-medium transition-colors ${
                  level === t.key ? 'bg-white text-navy-900 shadow-sm' : 'text-navy-500 hover:text-navy-800'
                }`}
              >
                {t.label}
              </button>
            ))}
          </div>
          <SearchInput value={query} onChange={setQuery} placeholder="Kullanıcı, işlem, IP..." />
        </div>
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-navy-50 text-left text-xs font-semibold uppercase tracking-wide text-navy-400">
                <th className="px-5 py-3">Seviye</th>
                <th className="px-5 py-3">Kullanıcı</th>
                <th className="px-5 py-3">İşlem</th>
                <th className="px-5 py-3">Nesne</th>
                <th className="px-5 py-3">Detay</th>
                <th className="px-5 py-3">IP</th>
                <th className="px-5 py-3">Tarih</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-navy-50">
              {filtered.map((l) => (
                <tr key={l.id} className="transition-colors hover:bg-navy-50/50">
                  <td className="px-5 py-3.5">
                    <Badge tone={levelTone[l.level]}>{levelLabel[l.level]}</Badge>
                  </td>
                  <td className="px-5 py-3.5 font-medium text-navy-900">{l.user}</td>
                  <td className="px-5 py-3.5 text-navy-600">{l.action}</td>
                  <td className="px-5 py-3.5 text-navy-600">{l.entity}</td>
                  <td className="px-5 py-3.5 text-navy-600">{l.detail}</td>
                  <td className="px-5 py-3.5 font-mono text-xs text-navy-500">{l.ip}</td>
                  <td className="px-5 py-3.5 whitespace-nowrap font-mono text-xs text-navy-400">{l.date}</td>
                </tr>
              ))}
              {filtered.length === 0 && (
                <tr>
                  <td colSpan={7} className="px-5 py-10 text-center text-muted-foreground">
                    Kayıt bulunamadı.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </Panel>
    </div>
  )
}
