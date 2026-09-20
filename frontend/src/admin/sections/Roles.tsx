import { useMemo, useState } from 'react'
import {
  Panel,
  SectionHead,
  Button,
  Badge,
  IconButton,
  Modal,
  Field,
  TextInput,
  Textarea,
  Check,
} from '../ui'
import { IconPlus, IconEdit, IconTrash } from '../icons'
import { adminRoles, adminPermissions, type AdminRole } from '../adminData'

export default function Roles() {
  const [roles, setRoles] = useState<AdminRole[]>(adminRoles)
  const [editing, setEditing] = useState<AdminRole | 'new' | null>(null)

  function remove(id: number) {
    setRoles((prev) => prev.filter((r) => r.id !== id))
  }

  return (
    <div>
      <SectionHead
        title="Roller"
        description="Rollere bağlı izinleri controller bazlı düzenleyin. Her rol bir izin kümesini temsil eder."
        action={
          <Button onClick={() => setEditing('new')}>
            <IconPlus className="h-4 w-4" /> Yeni Rol
          </Button>
        }
      />

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-3">
        {roles.map((r) => (
          <Panel key={r.id} className="flex flex-col p-5">
            <div className="flex items-start justify-between">
              <div>
                <h3 className="text-base font-bold text-navy-900">{r.name}</h3>
                <p className="mt-0.5 text-sm text-muted-foreground">{r.description}</p>
              </div>
              <Badge tone={r.active ? 'active' : 'passive'}>{r.active ? 'Aktif' : 'Pasif'}</Badge>
            </div>
            <div className="mt-4 flex gap-6 text-sm">
              <div>
                <span className="block text-lg font-bold text-navy-900">{r.permissionCount}</span>
                <span className="text-xs text-muted-foreground">İzin</span>
              </div>
              <div>
                <span className="block text-lg font-bold text-navy-900">{r.userCount}</span>
                <span className="text-xs text-muted-foreground">Kullanıcı</span>
              </div>
            </div>
            <div className="mt-4 flex gap-2 border-t border-navy-50 pt-4">
              <IconButton tone="edit" aria-label="Düzenle" onClick={() => setEditing(r)}>
                <IconEdit className="h-4 w-4" />
              </IconButton>
              <IconButton tone="delete" aria-label="Sil" onClick={() => remove(r.id)}>
                <IconTrash className="h-4 w-4" />
              </IconButton>
            </div>
          </Panel>
        ))}
      </div>

      {editing && (
        <RoleModal role={editing === 'new' ? null : editing} onClose={() => setEditing(null)} />
      )}
    </div>
  )
}

const crudOrder = ['View', 'Add', 'Update', 'Delete', 'Option', 'Preview']

function RoleModal({ role, onClose }: { role: AdminRole | null; onClose: () => void }) {
  const [name, setName] = useState(role?.name ?? '')
  const [description, setDescription] = useState(role?.description ?? '')
  const [active, setActive] = useState(role?.active ?? true)
  // Örnek: Admin rolü tüm izinlere sahip.
  const [granted, setGranted] = useState<Set<number>>(
    () => new Set(role?.name === 'Admin' ? adminPermissions.map((p) => p.id) : []),
  )

  // İzinleri controller'a göre grupla.
  const groups = useMemo(() => {
    const map = new Map<string, typeof adminPermissions>()
    for (const p of adminPermissions) {
      if (!map.has(p.controller)) map.set(p.controller, [])
      map.get(p.controller)!.push(p)
    }
    return [...map.entries()].map(([controller, perms]) => ({
      controller,
      perms: [...perms].sort((a, b) => crudOrder.indexOf(a.crudType) - crudOrder.indexOf(b.crudType)),
    }))
  }, [])

  function toggle(id: number) {
    setGranted((prev) => {
      const next = new Set(prev)
      next.has(id) ? next.delete(id) : next.add(id)
      return next
    })
  }

  function toggleGroup(ids: number[], on: boolean) {
    setGranted((prev) => {
      const next = new Set(prev)
      ids.forEach((id) => (on ? next.add(id) : next.delete(id)))
      return next
    })
  }

  return (
    <Modal
      title={role ? 'Rol Düzenle' : 'Yeni Rol'}
      onClose={onClose}
      wide
      footer={
        <>
          <Button variant="soft" onClick={onClose}>
            İptal
          </Button>
          <Button onClick={onClose}>Kaydet</Button>
        </>
      }
    >
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-[1fr_auto] sm:items-end">
        <Field label="Rol Adı">
          <TextInput value={name} onChange={(e) => setName(e.target.value)} />
        </Field>
        <div className="pb-2.5">
          <Check checked={active} onChange={setActive} label="Aktif" />
        </div>
      </div>
      <div className="mt-4">
        <Field label="Açıklama">
          <Textarea value={description} onChange={(e) => setDescription(e.target.value)} />
        </Field>
      </div>

      <div className="mt-5">
        <span className="mb-2 block text-sm font-medium text-navy-700">İzinler</span>
        <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
          {groups.map(({ controller, perms }) => {
            const ids = perms.map((p) => p.id)
            const allOn = ids.every((id) => granted.has(id))
            return (
              <div key={controller} className="overflow-hidden rounded-xl border border-navy-100">
                <div className="flex items-center justify-between bg-navy-800 px-4 py-2.5 text-white">
                  <Check checked={allOn} onChange={(on) => toggleGroup(ids, on)} label={
                    <span className="font-semibold text-white">{controller}</span>
                  } />
                </div>
                <div className="flex flex-wrap gap-x-5 gap-y-2 px-4 py-3">
                  {perms.map((p) => (
                    <Check
                      key={p.id}
                      checked={granted.has(p.id)}
                      onChange={() => toggle(p.id)}
                      label={p.crudType}
                    />
                  ))}
                </div>
              </div>
            )
          })}
        </div>
        <p className="mt-3 text-xs text-muted-foreground">
          Seçili izin sayısı: <span className="font-semibold text-navy-700">{granted.size}</span>
        </p>
      </div>
    </Modal>
  )
}
