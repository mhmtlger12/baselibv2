import { useState } from 'react'
import { Panel, SectionHead, Button, Badge, IconButton, Modal, Field, TextInput, Select, Check } from '../ui'
import { IconPlus, IconEdit, IconTrash } from '../icons'
import { adminMenus, adminPermissions, type AdminMenu } from '../adminData'

export default function Menus() {
  const [items, setItems] = useState<AdminMenu[]>([...adminMenus].sort((a, b) => a.order - b.order))
  const [editing, setEditing] = useState<AdminMenu | 'new' | null>(null)

  function remove(id: number) {
    setItems((prev) => prev.filter((m) => m.id !== id))
  }

  return (
    <div>
      <SectionHead
        title="Menüler"
        description="Panel menülerini, sıralamayı ve bağlı izinleri yönetin."
        action={
          <Button onClick={() => setEditing('new')}>
            <IconPlus className="h-4 w-4" /> Yeni Menü
          </Button>
        }
      />

      <Panel className="divide-y divide-navy-50 p-2">
        {items.map((m) => (
          <div key={m.id} className="flex items-center gap-3 rounded-xl px-3 py-3 transition-colors hover:bg-navy-50">
            <span className="flex h-7 w-7 items-center justify-center rounded-lg bg-teal-50 font-mono text-xs font-semibold text-teal-700">
              {m.order}
            </span>
            <div className="min-w-0">
              <div className="flex items-center gap-2">
                <span className="font-semibold text-navy-900">{m.name}</span>
                <Badge tone={m.active ? 'active' : 'passive'}>{m.active ? 'Aktif' : 'Pasif'}</Badge>
              </div>
              <span className="font-mono text-xs text-navy-400">{m.url}</span>
            </div>
            <div className="ml-auto hidden items-center gap-2 sm:flex">
              <Badge tone="admin">{m.permission}</Badge>
            </div>
            <div className="flex gap-2">
              <IconButton tone="edit" aria-label="Düzenle" onClick={() => setEditing(m)}>
                <IconEdit className="h-4 w-4" />
              </IconButton>
              <IconButton tone="delete" aria-label="Sil" onClick={() => remove(m.id)}>
                <IconTrash className="h-4 w-4" />
              </IconButton>
            </div>
          </div>
        ))}
      </Panel>

      {editing && <MenuModal menu={editing === 'new' ? null : editing} onClose={() => setEditing(null)} />}
    </div>
  )
}

function MenuModal({ menu, onClose }: { menu: AdminMenu | null; onClose: () => void }) {
  const [draft, setDraft] = useState(
    () =>
      menu ?? {
        name: '',
        url: '',
        icon: 'bi-circle',
        order: 0,
        permission: '',
        active: true,
      },
  )
  const set = (patch: Partial<typeof draft>) => setDraft((d) => ({ ...d, ...patch }))

  return (
    <Modal
      title={menu ? 'Menü Düzenle' : 'Yeni Menü'}
      onClose={onClose}
      footer={
        <>
          <Button variant="soft" onClick={onClose}>
            İptal
          </Button>
          <Button onClick={onClose}>Kaydet</Button>
        </>
      }
    >
      <div className="space-y-4">
        <Field label="Menü Adı">
          <TextInput value={draft.name} onChange={(e) => set({ name: e.target.value })} />
        </Field>
        <Field label="URL">
          <TextInput value={draft.url} onChange={(e) => set({ url: e.target.value })} placeholder="/Admin/Users" className="font-mono" />
        </Field>
        <div className="grid grid-cols-2 gap-4">
          <Field label="İkon (Bootstrap Icons)">
            <TextInput value={draft.icon} onChange={(e) => set({ icon: e.target.value })} placeholder="bi-person" className="font-mono" />
          </Field>
          <Field label="Sıra">
            <TextInput type="number" value={draft.order} onChange={(e) => set({ order: Number(e.target.value) })} />
          </Field>
        </div>
        <Field label="Bağlı İzin">
          <Select value={draft.permission} onChange={(e) => set({ permission: e.target.value })}>
            <option value="">İzin gerektirmez</option>
            {adminPermissions.map((p) => (
              <option key={p.id} value={p.code}>
                {p.code}
              </option>
            ))}
          </Select>
        </Field>
        <Check checked={draft.active} onChange={(v) => set({ active: v })} label="Aktif" />
      </div>
    </Modal>
  )
}
