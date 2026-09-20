import { useState } from 'react'
import { api } from '../../api/client'
import type { User } from '../../api/contracts'
import { Panel, SectionHead, Button, Badge, Field, TextInput } from '../ui'
import { IconRoles, IconLock, IconLogout } from '../icons'

const passwordHint = 'En az 12 karakter; büyük harf, küçük harf, rakam ve özel karakter içermelidir.'

export default function Profile({ user, onUserChange }: { user: User; onUserChange: (user: User) => void }) {
  const [currentPassword, setCurrentPassword] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [confirmation, setConfirmation] = useState('')
  const [saving, setSaving] = useState(false)
  const [formError, setFormError] = useState<string | null>(null)
  const rolePairs = user.roles.map((name, index) => ({ name, id: user.roleIds[index] }))
  const initials = `${user.firstName?.[0] ?? ''}${user.lastName?.[0] ?? user.username[0] ?? ''}`

  async function switchRole(roleId: number | undefined, roleName: string) {
    if (!roleId || !window.confirm(`'${roleName}' rolüne geçmek istediğinize emin misiniz?`)) return
    try {
      const result = await api.switchRole(roleId)
      onUserChange(result.user)
      // API yeni aktif rolü içeren bir JWT üretir. Önceki rolün bellekteki
      // ekran/verilerini taşımamak için uygulamayı bu yeni oturumla başlat.
      window.location.hash = 'admin'
      window.location.reload()
    } catch (cause) {
      window.alert(cause instanceof Error ? cause.message : 'Rol değiştirilemedi.')
    }
  }

  async function changePassword() {
    if (!currentPassword || !newPassword) {
      setFormError('Mevcut ve yeni şifre zorunludur.')
      return
    }
    if (newPassword !== confirmation) {
      setFormError('Yeni şifreler birbiriyle eşleşmiyor.')
      return
    }
    if (newPassword.length < 12 || newPassword.length > 128 || !/[A-Z]/.test(newPassword) || !/[a-z]/.test(newPassword) || !/\d/.test(newPassword) || !/[^A-Za-z0-9]/.test(newPassword)) {
      setFormError(passwordHint)
      return
    }

    setSaving(true)
    setFormError(null)
    try {
      await api.changePassword(currentPassword, newPassword)
      setCurrentPassword('')
      setNewPassword('')
      setConfirmation('')
    } catch (cause) {
      setFormError(cause instanceof Error ? cause.message : 'Şifre güncellenemedi.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <div>
      <SectionHead title="Hesap Ayarları" description="Profiliniz, rolleriniz ve güvenlik ayarlarınız API hesabınızdan alınır." />
      <div className="grid grid-cols-1 gap-4 lg:grid-cols-[340px_1fr]">
        <Panel className="h-fit p-6">
          <div className="flex flex-col items-center text-center">
            <span className="flex h-24 w-24 items-center justify-center rounded-full bg-gradient-to-br from-navy-600 to-teal-500 text-2xl font-bold text-white">{initials}</span>
            <h3 className="mt-4 text-lg font-bold text-navy-900">{user.firstName} {user.lastName}</h3>
            <p className="text-sm text-muted-foreground">{user.email}</p>
            <div className="mt-3 flex flex-wrap justify-center gap-1.5">
              {user.roles.map((role) => <Badge key={role} tone={role === user.activeRoleName ? 'active' : 'admin'}>{role}{role === user.activeRoleName && ' • aktif'}</Badge>)}
            </div>
          </div>
          <dl className="mt-6 divide-y divide-navy-50 text-sm">
            <Row label="Kullanıcı Adı" value={user.username} />
            <Row label="Departman" value={user.departmentName ?? '—'} />
            <Row label="Aktif Rol" value={user.activeRoleName ?? '—'} />
            <Row label="Kayıt Tarihi" value={formatDate(user.createdDate)} />
          </dl>
        </Panel>

        <div className="space-y-4">
          {rolePairs.length > 1 && (
            <Panel className="p-6">
              <div className="flex items-center gap-2"><IconRoles className="h-5 w-5 text-teal-600" /><h3 className="text-base font-bold text-navy-900">Rollerim</h3></div>
              <p className="mt-1 text-sm text-muted-foreground">Birden fazla rolünüz varsa hangi rol ile devam edeceğinizi seçebilirsiniz.</p>
              <ul className="mt-4 space-y-2">
                {rolePairs.map((role) => {
                  const active = role.id === user.activeRoleId
                  return <li key={role.name} className={`flex items-center gap-3 rounded-xl border px-4 py-3 ${active ? 'border-teal-300 bg-teal-50/60' : 'border-navy-100'}`}>
                    <IconRoles className="h-5 w-5 text-teal-600" />
                    <span className="font-semibold text-navy-900">{role.name}</span>
                    <div className="ml-auto">{active ? <Badge tone="active">Aktif Rol</Badge> : <Button variant="soft" onClick={() => switchRole(role.id, role.name)}><IconLogout className="h-4 w-4" /> Bu rol ile gir</Button>}</div>
                  </li>
                })}
              </ul>
            </Panel>
          )}

          <Panel className="p-6">
            <h3 className="text-base font-bold text-navy-900">Güvenlik Ayarları</h3>
            <div className="mt-4 flex items-start gap-3 rounded-xl bg-teal-50 p-4 text-sm text-teal-800"><IconLock className="mt-0.5 h-5 w-5 shrink-0 text-teal-600" /><p>Hesabınızın güvenliği için şifrenizi düzenli olarak değiştirmeniz önerilir.</p></div>
            <div className="mt-5 space-y-4">
              <Field label="Mevcut Şifre"><TextInput type="password" value={currentPassword} onChange={(event) => setCurrentPassword(event.target.value)} autoComplete="current-password" required /></Field>
              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <Field label="Yeni Şifre" hint={passwordHint}><TextInput type="password" value={newPassword} onChange={(event) => setNewPassword(event.target.value)} autoComplete="new-password" minLength={12} maxLength={128} required /></Field>
                <Field label="Yeni Şifre (Tekrar)" hint="Yeni şifreyle birebir eşleşmelidir."><TextInput type="password" value={confirmation} onChange={(event) => setConfirmation(event.target.value)} autoComplete="new-password" minLength={12} maxLength={128} required /></Field>
              </div>
            </div>
            {formError && <p role="alert" className="mt-4 rounded-xl bg-rose-50 px-3 py-2 text-sm text-rose-700">{formError}</p>}
            <div className="mt-5 flex justify-end"><Button disabled={saving} onClick={changePassword}>{saving ? 'Güncelleniyor…' : 'Şifreyi Güncelle'}</Button></div>
          </Panel>
        </div>
      </div>
    </div>
  )
}

function Row({ label, value }: { label: string; value: string }) {
  return <div className="flex items-center justify-between py-3"><dt className="text-muted-foreground">{label}</dt><dd className="font-semibold text-navy-900">{value}</dd></div>
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat('tr-TR', { dateStyle: 'medium' }).format(new Date(value))
}
