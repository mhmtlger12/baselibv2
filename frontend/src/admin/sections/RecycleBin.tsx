import { api } from '../../api/client'
import { useApiData } from '../../api/useApiData'
import { Panel, SectionHead, Badge, IconButton } from '../ui'
import { IconRestore, IconTrash } from '../icons'

export default function RecycleBin() {
  const { data, loading, error, reload } = useApiData(api.recycleBin)
  const items = data ?? []
  async function restore(type: string, id: number) {
    if (!window.confirm('Bu kaydı geri yüklemek istediğinize emin misiniz?')) return
    try { await api.restoreRecycleBinItem(type, id); await reload() } catch (cause) { window.alert(cause instanceof Error ? cause.message : 'Geri yükleme başarısız.') }
  }
  return <div><SectionHead title="Çöp Kutusu" description="Silinen kayıtları API üzerinden geri yükleyin." />
    {error ? <Panel className="p-6 text-rose-600">{error}</Panel> : loading ? <Panel className="p-6 text-muted-foreground">Yükleniyor…</Panel> : !items.length ? <Panel className="flex flex-col items-center gap-3 py-16 text-center"><IconTrash className="h-7 w-7 text-navy-300" /><p className="text-sm text-muted-foreground">Çöp kutusu boş.</p></Panel> : <Panel className="overflow-hidden"><div className="overflow-x-auto"><table className="w-full text-sm"><thead><tr className="border-b border-navy-50 text-left text-xs font-semibold uppercase tracking-wide text-navy-400"><th className="px-5 py-3">Tür</th><th className="px-5 py-3">Kayıt</th><th className="px-5 py-3">Silinme Tarihi</th><th className="px-5 py-3 text-right">İşlem</th></tr></thead><tbody className="divide-y divide-navy-50">{items.map((item) => <tr key={`${item.type}-${item.id}`}><td className="px-5 py-3.5"><Badge tone="admin">{item.typeName}</Badge></td><td className="px-5 py-3.5 font-medium text-navy-900">{item.name}</td><td className="px-5 py-3.5 font-mono text-xs text-navy-400">{item.deletedDate ? formatDate(item.deletedDate) : '—'}</td><td className="px-5 py-3.5 text-right"><IconButton tone="plain" aria-label="Geri yükle" onClick={() => restore(item.type, item.id)}><IconRestore className="h-4 w-4" /></IconButton></td></tr>)}</tbody></table></div></Panel>}
  </div>
}

function formatDate(value: string) { return new Intl.DateTimeFormat('tr-TR', { dateStyle: 'short', timeStyle: 'short' }).format(new Date(value)) }
