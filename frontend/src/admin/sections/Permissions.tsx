import { useMemo, useState } from 'react'
import { api } from '../../api/client'
import type { CrudActionType, Permission, PermissionInput } from '../../api/contracts'
import { useApiData } from '../../api/useApiData'
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
  Select,
  Check,
} from '../ui'
import { IconPlus, IconEdit, IconTrash } from '../icons'

const tones: Record<CrudActionType, 'info' | 'active' | 'warning' | 'danger' | 'admin' | 'passive'> = {
  1: 'info',
  2: 'active',
  3: 'warning',
  4: 'passive',
  5: 'admin',
  6: 'danger',
}
const crudTypes: CrudActionType[] = [1, 2, 3, 4, 5, 6]
const crudLabels: Record<CrudActionType, string> = {
  1: 'View',
  2: 'Add',
  3: 'Update',
  4: 'Preview',
  5: 'Option',
  6: 'Delete',
}

export default function Permissions() {
  const { data, loading, error, reload } = useApiData(api.permissions)
  const [query, setQuery] = useState('')
  const [editing, setEditing] = useState<Permission | 'new' | null>(null)

  const items = useMemo(() => {
    const search = query.toLocaleLowerCase('tr-TR')
    return (data ?? []).filter(
      (item) =>
        !search ||
        [item.name, item.code, item.controllerName, item.actionName]
          .join(' ')
          .toLocaleLowerCase('tr-TR')
          .includes(search),
    )
  }, [data, query])

  async function remove(id: number) {
    if (!window.confirm('Bu izni silmek istediğinize emin misiniz?')) return
    try {
      await api.deletePermission(id)
      await reload()
    } catch (cause) {
      window.alert(cause instanceof Error ? cause.message : 'Silme başarısız.')
    }
  }

  return (
    <div>
      <SectionHead
        title="İzinler"
        description="API controller izinlerini CRUD seviyesinde yönetin."
        action={
          <Button onClick={() => setEditing('new')}>
            <IconPlus className="h-4 w-4" /> Yeni İzin
          </Button>
        }
      />

      <Panel className="overflow-hidden">
        <div className="border-b border-navy-50 px-5 py-4">
          <SearchInput value={query} onChange={setQuery} placeholder="İzin kodu, controller, action…" />
        </div>
        {error ? (
          <p className="p-6 text-rose-600">{error}</p>
        ) : loading ? (
          <p className="p-6 text-muted-foreground">Yükleniyor…</p>
        ) : (
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
                {items.map((item) => (
                  <tr key={item.id}>
                    <td className="px-5 py-3.5 font-medium text-navy-900">{item.name}</td>
                    <td className="px-5 py-3.5">
                      <code className="rounded-md bg-navy-50 px-2 py-1 font-mono text-xs text-teal-700">
                        {item.code}
                      </code>
                    </td>
                    <td className="px-5 py-3.5 text-navy-600">{item.controllerName}</td>
                    <td className="px-5 py-3.5 text-navy-600">{item.actionName}</td>
                    <td className="px-5 py-3.5">
                      <Badge tone={tones[item.crudActionType]}>{crudLabels[item.crudActionType]}</Badge>
                    </td>
                    <td className="px-5 py-3.5">
                      <Badge tone={item.isActive ? 'active' : 'passive'}>
                        {item.isActive ? 'Aktif' : 'Pasif'}
                      </Badge>
                    </td>
                    <td className="px-5 py-3.5 text-right">
                      <IconButton tone="edit" aria-label="Düzenle" onClick={() => setEditing(item)}>
                        <IconEdit className="h-4 w-4" />
                      </IconButton>{' '}
                      <IconButton tone="delete" aria-label="Sil" onClick={() => remove(item.id)}>
                        <IconTrash className="h-4 w-4" />
                      </IconButton>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </Panel>

      {editing && (
        <PermissionModal
          permission={editing === 'new' ? null : editing}
          onClose={() => setEditing(null)}
          onSaved={reload}
        />
      )}
    </div>
  )
}

function PermissionModal({
  permission,
  onClose,
  onSaved,
}: {
  permission: Permission | null
  onClose: () => void
  onSaved: () => Promise<void>
}) {
  const [draft, setDraft] = useState<PermissionInput>(() =>
    permission
      ? {
          name: permission.name,
          code: permission.code,
          description: permission.description ?? '',
          controllerName: permission.controllerName,
          actionName: permission.actionName,
          crudActionType: permission.crudActionType,
          isActive: permission.isActive,
        }
      : {
          name: '',
          code: '',
          description: '',
          controllerName: '',
          actionName: '',
          crudActionType: 1,
          isActive: true,
        },
  )
  const [saving, setSaving] = useState(false)
  const [formError, setFormError] = useState<string | null>(null)
  const set = (patch: Partial<PermissionInput>) => setDraft((current) => ({ ...current, ...patch }))

  async function save() {
    const code = draft.code.trim()
    if (!draft.name.trim() || !code || !draft.controllerName.trim() || !draft.actionName.trim()) {
      setFormError('İzin adı, izin kodu, controller adı ve action adı zorunludur.')
      return
    }
    if (!/^[A-Za-z][A-Za-z0-9_]*$/.test(code)) {
      setFormError('İzin kodu harfle başlamalı; yalnızca harf, rakam ve alt çizgi içermelidir.')
      return
    }

    setSaving(true)
    setFormError(null)
    try {
      const payload = { ...draft, name: draft.name.trim(), code, controllerName: draft.controllerName.trim(), actionName: draft.actionName.trim() }
      if (permission) await api.updatePermission(permission.id, payload)
      else await api.createPermission(payload)
      await onSaved()
      onClose()
    } catch (cause) {
      setFormError(cause instanceof Error ? cause.message : 'Kayıt başarısız.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <Modal
      title={permission ? 'İzin Düzenle' : 'Yeni İzin'}
      onClose={onClose}
      footer={
        <>
          <Button variant="soft" onClick={onClose}>İptal</Button>
          <Button disabled={saving} onClick={save}>{saving ? 'Kaydediliyor…' : 'Kaydet'}</Button>
        </>
      }
    >
      <div className="space-y-4">
        <Field label="İzin Adı" hint="Panelde kullanıcıya gösterilecek açıklayıcı izin adı.">
          <TextInput value={draft.name} onChange={(event) => set({ name: event.target.value })} placeholder="Kullanıcı Listele" />
        </Field>
        <Field
          label="İzin Kodu"
          hint={
            <>
              Endpoint üzerindeki{' '}
              <code className="rounded bg-rose-50 px-1.5 py-0.5 font-mono font-semibold text-rose-700 ring-1 ring-inset ring-rose-200">
                RequirePermission
              </code>{' '}
              koduyla birebir aynı olmalıdır.
            </>
          }
        >
          <TextInput
            value={draft.code}
            onChange={(event) => set({ code: event.target.value })}
            placeholder="Users_Read"
            className="font-mono"
            maxLength={150}
            pattern="[A-Za-z][A-Za-z0-9_]*"
            spellCheck={false}
            autoComplete="off"
          />
        </Field>
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <Field label="Controller Adı" hint="Endpoint'in bulunduğu API controller adı.">
            <TextInput value={draft.controllerName} onChange={(event) => set({ controllerName: event.target.value })} placeholder="Users" />
          </Field>
          <Field label="Action Adı" hint="Controller içindeki action/metot adı.">
            <TextInput value={draft.actionName} onChange={(event) => set({ actionName: event.target.value })} placeholder="Index" />
          </Field>
        </div>
        <Field label="CRUD İşlem Tipi" hint="İznin temsil ettiği işlem türünü seçin.">
          <Select value={draft.crudActionType} onChange={(event) => set({ crudActionType: Number(event.target.value) as CrudActionType })}>
            {crudTypes.map((type) => <option key={type} value={type}>{crudLabels[type]}</option>)}
          </Select>
        </Field>
        <Field label="Açıklama" hint="Bu iznin ne amaçla kullanıldığını açıklayın.">
          <Textarea value={draft.description} onChange={(event) => set({ description: event.target.value })} />
        </Field>
        <Check checked={draft.isActive} onChange={(isActive) => set({ isActive })} label="Aktif" />
        {formError && <p role="alert" className="rounded-xl bg-rose-50 px-3 py-2 text-sm text-rose-700">{formError}</p>}
      </div>
    </Modal>
  )
}
