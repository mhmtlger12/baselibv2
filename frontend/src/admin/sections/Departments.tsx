import { useState } from 'react'
import { api } from '../../api/client'
import type { Department, DepartmentInput } from '../../api/contracts'
import { useApiData } from '../../api/useApiData'
import { Panel, SectionHead, Button, Badge, IconButton, Modal, Field, TextInput, Select, Check } from '../ui'
import { IconPlus, IconEdit, IconTrash, IconChevron } from '../icons'

type Node = Department & { children: Node[] }

function buildTree(items: Department[]): Node[] {
  const map = new Map(items.map((item) => [item.id, { ...item, children: [] as Node[] }]))
  const roots: Node[] = []
  for (const node of map.values()) {
    const parent = node.parentDepartmentId ? map.get(node.parentDepartmentId) : undefined
    parent ? parent.children.push(node) : roots.push(node)
  }
  return roots
}

export default function Departments() {
  const { data, loading, error, reload } = useApiData(api.departments)
  const [editing, setEditing] = useState<Department | 'new' | null>(null)
  const items = data ?? []

  async function remove(id: number) {
    if (!window.confirm('Bu departmanı silmek istediğinize emin misiniz?')) return
    try { await api.deleteDepartment(id); await reload() } catch (cause) { window.alert(cause instanceof Error ? cause.message : 'Silme başarısız.') }
  }

  return <div>
    <SectionHead title="Departmanlar" description="Organizasyon ağacını ve departman hiyerarşisini yönetin."
      action={<Button onClick={() => setEditing('new')}><IconPlus className="h-4 w-4" /> Yeni Departman</Button>} />
    {error ? <Panel className="p-6 text-rose-600">{error}</Panel> : loading ? <Panel className="p-6 text-muted-foreground">Yükleniyor…</Panel> :
      <Panel className="p-3"><ul>{buildTree(items).map((node) => <TreeRow key={node.id} node={node} depth={0} onEdit={setEditing} onRemove={remove} />)}</ul></Panel>}
    {editing && <DepartmentModal department={editing === 'new' ? null : editing} all={items} onClose={() => setEditing(null)} onSaved={reload} />}
  </div>
}

function TreeRow({ node, depth, onEdit, onRemove }: { node: Node; depth: number; onEdit: (item: Department) => void; onRemove: (id: number) => void }) {
  const [open, setOpen] = useState(true)
  const hasChildren = node.children.length > 0
  return <li>
    <div className="flex items-center gap-2 rounded-xl px-3 py-2.5 transition-colors hover:bg-navy-50" style={{ paddingLeft: depth * 24 + 12 }}>
      <button onClick={() => setOpen((value) => !value)} className={`flex h-6 w-6 items-center justify-center rounded-md text-navy-400 ${hasChildren ? '' : 'invisible'}`}><IconChevron className={`h-4 w-4 ${open ? '' : '-rotate-90'}`} /></button>
      <span className="flex h-8 w-8 items-center justify-center rounded-lg bg-navy-100 text-xs font-bold text-navy-700">{node.code}</span>
      <span className="font-semibold text-navy-900">{node.name}</span><Badge tone={node.isActive ? 'active' : 'passive'}>{node.isActive ? 'Aktif' : 'Pasif'}</Badge>
      <div className="ml-auto flex gap-2"><IconButton tone="edit" aria-label="Düzenle" onClick={() => onEdit(node)}><IconEdit className="h-4 w-4" /></IconButton><IconButton tone="delete" aria-label="Sil" onClick={() => onRemove(node.id)}><IconTrash className="h-4 w-4" /></IconButton></div>
    </div>
    {hasChildren && open && <ul>{node.children.map((child) => <TreeRow key={child.id} node={child} depth={depth + 1} onEdit={onEdit} onRemove={onRemove} />)}</ul>}
  </li>
}

function DepartmentModal({ department, all, onClose, onSaved }: { department: Department | null; all: Department[]; onClose: () => void; onSaved: () => Promise<void> }) {
  const [draft, setDraft] = useState<DepartmentInput>(() => department ? { name: department.name, code: department.code, parentDepartmentId: department.parentDepartmentId, isActive: department.isActive } : { name: '', code: '', parentDepartmentId: null, isActive: true })
  const [saving, setSaving] = useState(false)
  const set = (patch: Partial<DepartmentInput>) => setDraft((current) => ({ ...current, ...patch }))
  async function save() {
    if (!draft.name.trim() || !draft.code.trim()) return window.alert('Departman adı ve kodu zorunludur.')
    setSaving(true)
    try {
      if (department) await api.updateDepartment(department.id, draft)
      else await api.createDepartment({ name: draft.name, code: draft.code, parentDepartmentId: draft.parentDepartmentId })
      await onSaved(); onClose()
    } catch (cause) { window.alert(cause instanceof Error ? cause.message : 'Kayıt başarısız.') } finally { setSaving(false) }
  }
  return <Modal title={department ? 'Departman Düzenle' : 'Yeni Departman'} onClose={onClose} footer={<><Button variant="soft" onClick={onClose}>İptal</Button><Button disabled={saving} onClick={save}>{saving ? 'Kaydediliyor…' : 'Kaydet'}</Button></>}>
    <div className="space-y-4"><Field label="Departman Adı" hint="En fazla 150 karakter."><TextInput value={draft.name} onChange={(e) => set({ name: e.target.value })} maxLength={150} required /></Field><Field label="Departman Kodu" hint="Departmanı tanımlayan kısa kod; en fazla 50 karakter."><TextInput value={draft.code} onChange={(e) => set({ code: e.target.value })} maxLength={50} required /></Field><Field label="Üst Departman" hint="Seçmezseniz kök departman olarak oluşturulur."><Select value={draft.parentDepartmentId ?? ''} onChange={(e) => set({ parentDepartmentId: e.target.value ? Number(e.target.value) : null })}><option value="">Üst departman yok (Root)</option>{all.filter((item) => item.id !== department?.id).map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}</Select></Field><Check checked={draft.isActive} onChange={(isActive) => set({ isActive })} label="Aktif" /></div>
  </Modal>
}
