import { useMemo, useState } from 'react'
import {
  Panel,
  SectionHead,
  Button,
  Badge,
  IconButton,
  SearchInput,
  Modal,
  Field,
  TextInput,
  Textarea,
  Check,
} from '../ui'
import { IconPlus, IconEdit, IconTrash } from '../icons'
import { adminPermissions, type AdminPermission, type CrudType } from '../adminData'

const crudTones: Record<CrudType, 'info' | 'active' | 'warning' | 'danger' | 'admin' | 'passive'> = {
  View: 'info',
  Add: 'active',
  Update: 'warning',
  Delete: 'danger',
  Option: 'admin',
  Preview: 'passive',
}

export default function Permissions() {
  const [items, setItems] = useState<AdminPermission[]>(adminPermissions)
  const [query, setQuery] = useState('')
  const [editing, setEditing] = useState<AdminPermission | 'new' | null>(null)

  const filtered = useMemo(() => {
    const q = query.trim().toLowerCase()
    if (!q) return items
    return items.filter((p) =>
      [p.name, p.code, p.controller, p.action].join(' ').toLowerCase().includes(q),
    )
  }, [items, query])

  function remove(id: number) {
    setItems((prev) => prev.filter((p) => p.id !== id))
  }

  return (
    <div>
      <SectionHead
        title="İzinler"
        description="API controller izinlerini CRUD seviyesinde yönetin. İzin kodu, endpoint üzerindeki yetki koduyla birebir eşleşmelidir."
        action={
          <Button onClick={() => setEditing('new')}>
            <IconPlus className="h-4 w-4" /> Yeni İzin
          </Button>
        }
      />

      <Panel className="overflow-hidden">
        <div className="border-b border-navy-50 px-5 py-4">
          <SearchInput value={query} onChange={setQuery} placeholder="İzin kodu, controller, action..." />
        </div>
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-navy-50 text-left text-xs font-semibold uppercase tracking-wide text-navy-400">
                <th className="px-5 py-3">Adı</th>
                <th className="px-5 py-3">İzin Kodu</th>
                <th className="px-5 py-3">Controller</th>
                <th className="px-5 py-3">Action</th>
                <th className="px-5 py-3">CRUD</th>
                <th className="px-5 py-3">Durum</th>
                <th className="px-5 py-3 text-right">İşlemler</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-navy-50">
              {filtered.map((p) => (
                <tr key={p.id} className="transition-colors hover:bg-navy-50/50">
                  <td className="px-5 py-3.5 font-medium text-navy-900">{p.name}</td>
                  <td className="px-5 py-3.5">
                    <code className="rounded-md bg-navy-50 px-2 py-1 font-mono text-xs text-teal-700">
                      {p.code}
                    </code>
                  </td>
                  <td className="px-5 py-3.5 text-navy-600">{p.controller}</td>
                  <td className="px-5 py-3.5 text-navy-600">{p.action}</td>
                  <td className="px-5 py-3.5">
                    <Badge tone={crudTones[p.crudType]}>{p.crudType}</Badge>
                  </td>
                  <td className="px-5 py-3.5">
                    <Badge tone={p.active ? 'active' : 'passive'}>{p.active ? 'Aktif' : 'Pasif'}</Badge>
                  </td>
                  <td className="px-5 py-3.5">
                    <div className="flex justify-end gap-2">
                      <IconButton tone="edit" aria-label="Düzenle" onClick={() => setEditing(p)}>
                        <IconEdit className="h-4 w-4" />
                      </IconButton>
                      <IconButton tone="delete" aria-label="Sil" onClick={() => remove(p.id)}>
                        <IconTrash className="h-4 w-4" />
                      </IconButton>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </Panel>

      {editing && (
        <PermissionModal
          permission={editing === 'new' ? null : editing}
          onClose={() => setEditing(null)}
        />
      )}
    </div>
  )
}

const crudTypes: CrudType[] = ['View', 'Add', 'Update', 'Delete', 'Option', 'Preview']

function PermissionModal({
  permission,
  onClose,
}: {
  permission: AdminPermission | null
  onClose: () => void
}) {
  const [draft, setDraft] = useState(
    () =>
      permission ?? {
        name: '',
        code: '',
        controller: '',
        action: '',
        description: '',
        crudType: 'View' as CrudType,
        active: true,
      },
  )
  const set = (patch: Partial<typeof draft>) => setDraft((d) => ({ ...d, ...patch }))

  return (
    <Modal
      title={permission ? 'İzin Düzenle' : 'Yeni İzin'}
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
        <Field label="İzin Adı">
          <TextInput
            value={draft.name}
            onChange={(e) => set({ name: e.target.value })}
            placeholder="Kullanıcı Listele"
          />
        </Field>
        <Field label="İzin Kodu" hint="Endpoint üzerindeki RequirePermission koduyla birebir aynı olmalıdır.">
          <TextInput
            value={draft.code}
            onChange={(e) => set({ code: e.target.value })}
            placeholder="Users_Read"
            className="font-mono"
          />
        </Field>
        <div className="grid grid-cols-2 gap-4">
          <Field label="Controller Adı">
            <TextInput value={draft.controller} onChange={(e) => set({ controller: e.target.value })} placeholder="Users" />
          </Field>
          <Field label="Action Adı">
            <TextInput value={draft.action} onChange={(e) => set({ action: e.target.value })} placeholder="List" />
          </Field>
        </div>
        <div>
          <span className="mb-1.5 block text-sm font-medium text-navy-700">CRUD İşlem Tipi</span>
          <div className="flex flex-wrap gap-2">
            {crudTypes.map((t) => (
              <button
                key={t}
                onClick={() => set({ crudType: t })}
                className={`rounded-lg px-3 py-1.5 text-sm font-medium transition-colors ${
                  draft.crudType === t
                    ? 'bg-navy-700 text-white'
                    : 'bg-navy-50 text-navy-600 hover:bg-navy-100'
                }`}
              >
                {t}
              </button>
            ))}
          </div>
        </div>
        <Field label="Açıklama">
          <Textarea value={draft.description} onChange={(e) => set({ description: e.target.value })} />
        </Field>
        <Check checked={draft.active} onChange={(v) => set({ active: v })} label="Aktif" />
      </div>
    </Modal>
  )
}
