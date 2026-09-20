import { useState } from 'react'
import { Panel, SectionHead, Button, Badge, IconButton, Modal, Field, TextInput, Select, Check } from '../ui'
import { IconPlus, IconEdit, IconTrash, IconChevron } from '../icons'
import { adminDepartments, type AdminDepartment } from '../adminData'

type Node = AdminDepartment & { children: Node[] }

function buildTree(list: AdminDepartment[]): Node[] {
  const map = new Map<number, Node>()
  list.forEach((d) => map.set(d.id, { ...d, children: [] }))
  const roots: Node[] = []
  map.forEach((node) => {
    if (node.parentId && map.has(node.parentId)) map.get(node.parentId)!.children.push(node)
    else roots.push(node)
  })
  return roots
}

export default function Departments() {
  const [items, setItems] = useState<AdminDepartment[]>(adminDepartments)
  const [editing, setEditing] = useState<AdminDepartment | 'new' | null>(null)
  const tree = buildTree(items)

  function remove(id: number) {
    setItems((prev) => prev.filter((d) => d.id !== id && d.parentId !== id))
  }

  return (
    <div>
      <SectionHead
        title="Departmanlar"
        description="Organizasyon ağacını ve departman hiyerarşisini yönetin."
        action={
          <Button onClick={() => setEditing('new')}>
            <IconPlus className="h-4 w-4" /> Yeni Departman
          </Button>
        }
      />

      <Panel className="p-3">
        <ul>
          {tree.map((node) => (
            <TreeRow key={node.id} node={node} depth={0} onEdit={setEditing} onRemove={remove} />
          ))}
        </ul>
      </Panel>

      {editing && (
        <DepartmentModal
          department={editing === 'new' ? null : editing}
          all={items}
          onClose={() => setEditing(null)}
        />
      )}
    </div>
  )
}

function TreeRow({
  node,
  depth,
  onEdit,
  onRemove,
}: {
  node: Node
  depth: number
  onEdit: (d: AdminDepartment) => void
  onRemove: (id: number) => void
}) {
  const [open, setOpen] = useState(true)
  const hasChildren = node.children.length > 0

  return (
    <li>
      <div
        className="flex items-center gap-2 rounded-xl px-3 py-2.5 transition-colors hover:bg-navy-50"
        style={{ paddingLeft: depth * 24 + 12 }}
      >
        <button
          onClick={() => setOpen((v) => !v)}
          className={`flex h-6 w-6 items-center justify-center rounded-md text-navy-400 transition-transform hover:bg-navy-100 ${
            open ? '' : '-rotate-90'
          } ${hasChildren ? '' : 'invisible'}`}
          aria-label={open ? 'Kapat' : 'Aç'}
        >
          <IconChevron className="h-4 w-4" />
        </button>
        <span className="flex h-8 w-8 items-center justify-center rounded-lg bg-navy-100 text-xs font-bold text-navy-700">
          {node.code}
        </span>
        <span className="font-semibold text-navy-900">{node.name}</span>
        <Badge tone={node.active ? 'active' : 'passive'}>{node.active ? 'Aktif' : 'Pasif'}</Badge>
        <div className="ml-auto flex gap-2">
          <IconButton tone="edit" aria-label="Düzenle" onClick={() => onEdit(node)}>
            <IconEdit className="h-4 w-4" />
          </IconButton>
          <IconButton tone="delete" aria-label="Sil" onClick={() => onRemove(node.id)}>
            <IconTrash className="h-4 w-4" />
          </IconButton>
        </div>
      </div>
      {hasChildren && open && (
        <ul>
          {node.children.map((child) => (
            <TreeRow key={child.id} node={child} depth={depth + 1} onEdit={onEdit} onRemove={onRemove} />
          ))}
        </ul>
      )}
    </li>
  )
}

function DepartmentModal({
  department,
  all,
  onClose,
}: {
  department: AdminDepartment | null
  all: AdminDepartment[]
  onClose: () => void
}) {
  const [draft, setDraft] = useState(
    () => department ?? { name: '', code: '', active: true, parentId: null as number | null },
  )
  const set = (patch: Partial<typeof draft>) => setDraft((d) => ({ ...d, ...patch }))

  return (
    <Modal
      title={department ? 'Departman Düzenle' : 'Yeni Departman'}
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
        <Field label="Departman Adı">
          <TextInput value={draft.name} onChange={(e) => set({ name: e.target.value })} />
        </Field>
        <Field label="Departman Kodu">
          <TextInput value={draft.code} onChange={(e) => set({ code: e.target.value })} />
        </Field>
        <Field label="Üst Departman">
          <Select
            value={draft.parentId ?? ''}
            onChange={(e) => set({ parentId: e.target.value ? Number(e.target.value) : null })}
          >
            <option value="">Üst departman yok</option>
            {all
              .filter((d) => d.id !== department?.id)
              .map((d) => (
                <option key={d.id} value={d.id}>
                  {d.name}
                </option>
              ))}
          </Select>
        </Field>
        <Check checked={draft.active} onChange={(v) => set({ active: v })} label="Aktif" />
      </div>
    </Modal>
  )
}
