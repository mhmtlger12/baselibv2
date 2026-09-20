import { useState } from 'react'
import { api } from '../../api/client'
import type { Menu, MenuInput, Permission } from '../../api/contracts'
import { useApiData } from '../../api/useApiData'
import { Panel, SectionHead, Button, Badge, IconButton, Modal, Field, TextInput, Select, Check } from '../ui'
import { IconPlus, IconEdit, IconTrash } from '../icons'

export default function Menus() {
  const menus = useApiData(api.menus)
  const permissions = useApiData(api.permissions)
  const [editing, setEditing] = useState<Menu | 'new' | null>(null)
  const items = [...(menus.data ?? [])].sort((a, b) => a.order - b.order)

  async function remove(id: number) {
    if (!window.confirm('Bu menüyü silmek istediğinize emin misiniz?')) return
    try {
      await api.deleteMenu(id)
      await menus.reload()
    } catch (cause) {
      window.alert(cause instanceof Error ? cause.message : 'Silme başarısız.')
    }
  }

  return (
    <div>
      <SectionHead
        title="Menüler"
        description="Panel menülerini, sıralamayı, hiyerarşiyi ve bağlı izinleri yönetin."
        action={<Button onClick={() => setEditing('new')}><IconPlus className="h-4 w-4" /> Yeni Menü</Button>}
      />
      {menus.error ? (
        <Panel className="p-6 text-rose-600">{menus.error}</Panel>
      ) : menus.loading ? (
        <Panel className="p-6 text-muted-foreground">Yükleniyor…</Panel>
      ) : (
        <Panel className="divide-y divide-navy-50 p-2">
          {items.map((item) => (
            <div key={item.id} className="flex items-center gap-3 rounded-xl px-3 py-3 hover:bg-navy-50">
              <span className="flex h-7 w-7 items-center justify-center rounded-lg bg-teal-50 font-mono text-xs font-semibold text-teal-700">{item.order}</span>
              <div className="min-w-0">
                <div className="flex items-center gap-2">
                  <span className="font-semibold text-navy-900">{item.name}</span>
                  <Badge tone={item.isActive ? 'active' : 'passive'}>{item.isActive ? 'Aktif' : 'Pasif'}</Badge>
                </div>
                <span className="font-mono text-xs text-navy-400">{item.url}</span>
              </div>
              <div className="ml-auto hidden items-center gap-2 sm:flex">
                {item.parentId && <Badge tone="passive">Alt menü</Badge>}
                <Badge tone="admin">{item.permissionCode ?? 'İzin yok'}</Badge>
              </div>
              <div className="flex gap-2">
                <IconButton tone="edit" aria-label="Düzenle" onClick={() => setEditing(item)}><IconEdit className="h-4 w-4" /></IconButton>
                <IconButton tone="delete" aria-label="Sil" onClick={() => remove(item.id)}><IconTrash className="h-4 w-4" /></IconButton>
              </div>
            </div>
          ))}
        </Panel>
      )}
      {editing && (
        <MenuModal
          menu={editing === 'new' ? null : editing}
          allMenus={items}
          permissions={permissions.data ?? []}
          onClose={() => setEditing(null)}
          onSaved={menus.reload}
        />
      )}
    </div>
  )
}

function MenuModal({
  menu,
  allMenus,
  permissions,
  onClose,
  onSaved,
}: {
  menu: Menu | null
  allMenus: Menu[]
  permissions: Permission[]
  onClose: () => void
  onSaved: () => Promise<void>
}) {
  const [draft, setDraft] = useState<MenuInput>(() =>
    menu
      ? { name: menu.name, url: menu.url ?? '', icon: menu.icon ?? '', parentId: menu.parentId, order: menu.order, permissionId: menu.permissionId, isActive: menu.isActive }
      : { name: '', url: '', icon: '', parentId: null, order: 0, permissionId: null, isActive: true },
  )
  const [saving, setSaving] = useState(false)
  const [formError, setFormError] = useState<string | null>(null)
  const set = (patch: Partial<MenuInput>) => setDraft((current) => ({ ...current, ...patch }))

  async function save() {
    if (!draft.name.trim()) {
      setFormError('Menü adı zorunludur.')
      return
    }
    if (!Number.isInteger(draft.order) || draft.order < 0 || draft.order > 10000) {
      setFormError('Sıra, 0 ile 10000 arasında tam sayı olmalıdır.')
      return
    }
    setSaving(true)
    setFormError(null)
    try {
      const payload = { ...draft, name: draft.name.trim(), url: draft.url.trim(), icon: draft.icon.trim() }
      if (menu) await api.updateMenu(menu.id, payload)
      else await api.createMenu({ name: payload.name, url: payload.url, icon: payload.icon, parentId: payload.parentId, order: payload.order, permissionId: payload.permissionId })
      await onSaved()
      onClose()
    } catch (cause) {
      setFormError(cause instanceof Error ? cause.message : 'Kayıt başarısız.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <Modal title={menu ? 'Menü Düzenle' : 'Yeni Menü'} onClose={onClose} footer={<><Button variant="soft" onClick={onClose}>İptal</Button><Button disabled={saving} onClick={save}>{saving ? 'Kaydediliyor…' : 'Kaydet'}</Button></>}>
      <div className="space-y-4">
        <Field label="Menü Adı" hint="Yönetim panelinde gösterilecek menü başlığı.">
          <TextInput value={draft.name} onChange={(event) => set({ name: event.target.value })} maxLength={150} placeholder="Kullanıcılar" />
        </Field>
        <Field label="URL" hint="İçerik veya yönlendirme yolu. Boş bırakılabilir.">
          <TextInput value={draft.url} onChange={(event) => set({ url: event.target.value })} placeholder="/Admin/Users" className="font-mono" maxLength={2048} />
        </Field>
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <Field label="İkon (Bootstrap Icons)" hint="Örnek: bi-person">
            <TextInput value={draft.icon} onChange={(event) => set({ icon: event.target.value })} placeholder="bi-person" className="font-mono" maxLength={100} />
          </Field>
          <Field label="Sıra" hint="0–10000 arası küçük değer önce görünür.">
            <TextInput type="number" min={0} max={10000} value={draft.order} onChange={(event) => set({ order: Number(event.target.value) })} />
          </Field>
        </div>
        <Field label="Üst Menü" hint="Bu menüyü başka bir menünün altında göstermek için seçin.">
          <Select value={draft.parentId ?? ''} onChange={(event) => set({ parentId: event.target.value ? Number(event.target.value) : null })}>
            <option value="">Üst menü yok (Root)</option>
            {allMenus.filter((item) => item.id !== menu?.id).map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
          </Select>
        </Field>
        <Field label="Bağlı İzin" hint="İzin seçilirse menü yalnızca bu izne erişebilen kullanıcılara görünür.">
          <Select value={draft.permissionId ?? ''} onChange={(event) => set({ permissionId: event.target.value ? Number(event.target.value) : null })}>
            <option value="">İzin gerektirmez</option>
            {permissions.map((permission) => <option key={permission.id} value={permission.id}>{permission.code}</option>)}
          </Select>
        </Field>
        <Check checked={draft.isActive} onChange={(isActive) => set({ isActive })} label="Aktif" />
        {formError && <p role="alert" className="rounded-xl bg-rose-50 px-3 py-2 text-sm text-rose-700">{formError}</p>}
      </div>
    </Modal>
  )
}
