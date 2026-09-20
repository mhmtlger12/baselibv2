import { useMemo, useState } from 'react'
import { api } from '../../api/client'
import { useApiData } from '../../api/useApiData'
import { Panel, SectionHead, SearchInput } from '../ui'

export default function AuditLogs() {
  const { data, loading, error } = useApiData(api.auditLogs)
  const [query, setQuery] = useState('')
  const items = useMemo(() => {
    const search = query.trim().toLocaleLowerCase('tr-TR')
    return (data ?? []).filter((item) => !search || [item.username, item.action, item.controller, item.route, item.details].join(' ').toLocaleLowerCase('tr-TR').includes(search))
  }, [data, query])
  return <div><SectionHead title="Sistem Hareketleri" description="Kullanıcı ve sistem işlemlerinin denetim kayıtları." />
    <Panel className="overflow-hidden"><div className="border-b border-navy-50 px-5 py-4"><SearchInput value={query} onChange={setQuery} placeholder="Kullanıcı, işlem, controller…" /></div>
      {error ? <p className="p-6 text-rose-600">{error}</p> : loading ? <p className="p-6 text-muted-foreground">Yükleniyor…</p> : <div className="overflow-x-auto"><table className="w-full text-sm"><thead><tr className="border-b border-navy-50 text-left text-xs font-semibold uppercase tracking-wide text-navy-400"><th className="px-5 py-3">Kullanıcı</th><th className="px-5 py-3">İşlem</th><th className="px-5 py-3">Controller</th><th className="px-5 py-3">Rota</th><th className="px-5 py-3">Detay</th><th className="px-5 py-3">Tarih</th></tr></thead><tbody className="divide-y divide-navy-50">{items.map((item) => <tr key={item.id} className="hover:bg-navy-50/50"><td className="px-5 py-3.5 font-medium text-navy-900">{item.username ?? 'Sistem'}</td><td className="px-5 py-3.5 text-navy-600">{item.action}</td><td className="px-5 py-3.5 text-navy-600">{item.controller}</td><td className="px-5 py-3.5 font-mono text-xs text-navy-500">{item.route}</td><td className="max-w-sm truncate px-5 py-3.5 text-navy-600" title={item.details ?? ''}>{item.details ?? '—'}</td><td className="whitespace-nowrap px-5 py-3.5 font-mono text-xs text-navy-400">{formatDate(item.createdDate)}</td></tr>)}{!items.length && <tr><td colSpan={6} className="px-5 py-10 text-center text-muted-foreground">Kayıt bulunamadı.</td></tr>}</tbody></table></div>}
    </Panel>
  </div>
}

function formatDate(value: string) { return new Intl.DateTimeFormat('tr-TR', { dateStyle: 'short', timeStyle: 'short' }).format(new Date(value)) }
