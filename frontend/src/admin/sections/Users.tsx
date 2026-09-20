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
  Select,
  Check,
} from '../ui'
import { IconPlus, IconEdit, IconTrash } from '../icons'
import { adminUsers, adminDepartments, adminRoles, type AdminUser } from '../adminData'

const emptyDraft = {
  firstName: '',
  lastName: '',
  username: '',
  email: '',
  phone: '',
  department: '',
  roles: [] as string[],
  active: true,
}

export default function Users() {
  const [users, setUsers] = useState<AdminUser[]>(adminUsers)
  const [query, setQuery] = useState('')
  const [editing, setEditing] = useState<AdminUser | 'new' | null>(null)

  const filtered = useMemo(() => {
    const q = query.trim().toLowerCase()
    if (!q) return users
    return users.filter((u) =>
      [u.firstName, u.lastName, u.username, u.email, u.department].join(' ').toLowerCase().includes(q),
    )
  }, [users, query])

  function remove(id: number) {
    setUsers((prev) => prev.filter((u) => u.id !== id))
  }

  function save(draft: typeof emptyDraft, id?: number) {
    if (id) {
      setUsers((prev) => prev.map((u) => (u.id === id ? { ...u, ...draft } : u)))
    } else {
      setUsers((prev) => [
        ...prev,
        { ...draft, id: Math.max(0, ...prev.map((u) => u.id)) + 1, lastLogin: '—' },
      ])
    }
    setEditing(null)
  }

  return (
    <div>
      <SectionHead
        title="Kullanıcılar"
        description="Kullanıcı hesaplarını, departmanlarını ve rol atamalarını yönetin."
        action={
          <Button onClick={() => setEditing('new')}>
            <IconPlus className="h-4 w-4" /> Yeni Kullanıcı
          </Button>
        }
      />

      <Panel className="overflow-hidden">
        <div className="border-b border-navy-50 px-5 py-4">
          <SearchInput value={query} onChange={setQuery} placeholder="İsim, kullanıcı adı, e-posta..." />
        </div>
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-navy-50 text-left text-xs font-semibold uppercase tracking-wide text-navy-400">
                <th className="px-5 py-3">Ad Soyad</th>
                <th className="px-5 py-3">Kullanıcı Adı</th>
                <th className="px-5 py-3">E-posta</th>
                <th className="px-5 py-3">Departman</th>
                <th className="px-5 py-3">Roller</th>
                <th className="px-5 py-3">Durum</th>
                <th className="px-5 py-3 text-right">İşlemler</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-navy-50">
              {filtered.map((u) => (
                <tr key={u.id} className="transition-colors hover:bg-navy-50/50">
                  <td className="px-5 py-3.5">
                    <div className="flex items-center gap-3">
                      <span className="flex h-9 w-9 items-center justify-center rounded-full bg-gradient-to-br from-navy-600 to-teal-600 text-xs font-bold text-white">
                        {u.firstName[0]}
                        {u.lastName[0]}
                      </span>
                      <span className="font-medium text-navy-900">
                        {u.firstName} {u.lastName}
                      </span>
                    </div>
                  </td>
                  <td className="px-5 py-3.5 text-navy-600">{u.username}</td>
                  <td className="px-5 py-3.5 text-navy-600">{u.email}</td>
                  <td className="px-5 py-3.5 text-navy-600">{u.department}</td>
                  <td className="px-5 py-3.5">
                    <div className="flex flex-wrap gap-1">
                      {u.roles.map((r) => (
                        <Badge key={r} tone="admin">
                          {r}
                        </Badge>
                      ))}
                    </div>
                  </td>
                  <td className="px-5 py-3.5">
                    <Badge tone={u.active ? 'active' : 'passive'}>{u.active ? 'Aktif' : 'Pasif'}</Badge>
                  </td>
                  <td className="px-5 py-3.5">
                    <div className="flex justify-end gap-2">
                      <IconButton tone="edit" aria-label="Düzenle" onClick={() => setEditing(u)}>
                        <IconEdit className="h-4 w-4" />
                      </IconButton>
                      <IconButton tone="delete" aria-label="Sil" onClick={() => remove(u.id)}>
                        <IconTrash className="h-4 w-4" />
                      </IconButton>
                    </div>
                  </td>
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

      {editing && (
        <UserModal
          user={editing === 'new' ? null : editing}
          onClose={() => setEditing(null)}
          onSave={save}
        />
      )}
    </div>
  )
}

function UserModal({
  user,
  onClose,
  onSave,
}: {
  user: AdminUser | null
  onClose: () => void
  onSave: (draft: typeof emptyDraft, id?: number) => void
}) {
  const [draft, setDraft] = useState(() => (user ? { ...user } : { ...emptyDraft }))
  const set = <K extends keyof typeof emptyDraft>(key: K, value: (typeof emptyDraft)[K]) =>
    setDraft((d) => ({ ...d, [key]: value }))

  function toggleRole(name: string) {
    set('roles', draft.roles.includes(name) ? draft.roles.filter((r) => r !== name) : [...draft.roles, name])
  }

  return (
    <Modal
      title={user ? 'Kullanıcı Düzenle' : 'Yeni Kullanıcı'}
      onClose={onClose}
      footer={
        <>
          <Button variant="soft" onClick={onClose}>
            İptal
          </Button>
          <Button onClick={() => onSave(draft, user?.id)}>Kaydet</Button>
        </>
      }
    >
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <Field label="Ad">
          <TextInput value={draft.firstName} onChange={(e) => set('firstName', e.target.value)} />
        </Field>
        <Field label="Soyad">
          <TextInput value={draft.lastName} onChange={(e) => set('lastName', e.target.value)} />
        </Field>
        <Field label="Kullanıcı Adı">
          <TextInput value={draft.username} onChange={(e) => set('username', e.target.value)} />
        </Field>
        <Field label="E-posta">
          <TextInput type="email" value={draft.email} onChange={(e) => set('email', e.target.value)} />
        </Field>
        <Field label="Telefon">
          <TextInput value={draft.phone} onChange={(e) => set('phone', e.target.value)} />
        </Field>
        <Field label="Departman">
          <Select value={draft.department} onChange={(e) => set('department', e.target.value)}>
            <option value="">Seçiniz</option>
            {adminDepartments.map((d) => (
              <option key={d.id} value={d.name}>
                {d.name}
              </option>
            ))}
          </Select>
        </Field>
        <div className="sm:col-span-2">
          <Field label="Şifre" hint="Düzenlemede boş bırakılırsa mevcut şifre korunur.">
            <TextInput type="password" placeholder="••••••••" />
          </Field>
        </div>
        <div className="sm:col-span-2">
          <span className="mb-1.5 block text-sm font-medium text-navy-700">Roller</span>
          <div className="flex flex-wrap gap-4">
            {adminRoles.map((r) => (
              <Check
                key={r.id}
                checked={draft.roles.includes(r.name)}
                onChange={() => toggleRole(r.name)}
                label={r.name}
              />
            ))}
          </div>
        </div>
        <div className="sm:col-span-2">
          <Check checked={draft.active} onChange={(v) => set('active', v)} label="Hesap aktif" />
        </div>
      </div>
    </Modal>
  )
}
