import { useState } from 'react'
import { api } from '../../api/client'
import type { Setting } from '../../api/contracts'
import { useApiData } from '../../api/useApiData'
import { Panel, SectionHead, Button, IconButton, Modal, Field, Textarea } from '../ui'
import { IconEdit } from '../icons'

export default function Settings() {
  const { data, loading, error, reload } = useApiData(api.settings)
  const [editing, setEditing] = useState<Setting | null>(null)
  return <div><SectionHead title="Sistem Ayarları" description="API’de tanımlı sistem ayarlarını yönetin." />
    {error ? <Panel className="p-6 text-rose-600">{error}</Panel> : loading ? <Panel className="p-6 text-muted-foreground">Yükleniyor…</Panel> : <Panel className="overflow-hidden"><table className="w-full text-sm"><thead><tr className="border-b border-navy-50 text-left text-xs font-semibold uppercase tracking-wide text-navy-400"><th className="px-5 py-3">Anahtar</th><th className="px-5 py-3">Değer</th><th className="px-5 py-3">Açıklama</th><th className="px-5 py-3 text-right">İşlem</th></tr></thead><tbody className="divide-y divide-navy-50">{(data ?? []).map((setting) => <tr key={setting.id}><td className="px-5 py-3.5 font-mono text-xs font-semibold text-teal-700">{setting.key}</td><td className="max-w-md truncate px-5 py-3.5 text-navy-700" title={setting.value}>{setting.value}</td><td className="px-5 py-3.5 text-navy-500">{setting.description}</td><td className="px-5 py-3.5 text-right"><IconButton tone="edit" aria-label="Düzenle" onClick={() => setEditing(setting)}><IconEdit className="h-4 w-4" /></IconButton></td></tr>)}</tbody></table></Panel>}
    {editing && <SettingModal setting={editing} onClose={() => setEditing(null)} onSaved={reload} />}
  </div>
}

function SettingModal({ setting, onClose, onSaved }: { setting: Setting; onClose: () => void; onSaved: () => Promise<void> }) {
  const [value, setValue] = useState(setting.value); const [saving, setSaving] = useState(false)
  async function save() { setSaving(true); try { await api.updateSetting(setting.id, value); await onSaved(); onClose() } catch (cause) { window.alert(cause instanceof Error ? cause.message : 'Kayıt başarısız.') } finally { setSaving(false) } }
  return <Modal title={`${setting.key} ayarını düzenle`} onClose={onClose} footer={<><Button variant="soft" onClick={onClose}>İptal</Button><Button disabled={saving} onClick={save}>{saving ? 'Kaydediliyor…' : 'Kaydet'}</Button></>}><Field label="Yeni Değer" hint={<>{setting.description}{setting.description && ' '}Eğer değer açık/kapalı tipindeyse <code className="rounded bg-rose-50 px-1.5 py-0.5 font-mono font-semibold text-rose-700 ring-1 ring-inset ring-rose-200">true</code> veya <code className="rounded bg-rose-50 px-1.5 py-0.5 font-mono font-semibold text-rose-700 ring-1 ring-inset ring-rose-200">false</code> yazın.</>}><Textarea value={value} onChange={(event) => setValue(event.target.value)} /></Field></Modal>
}
