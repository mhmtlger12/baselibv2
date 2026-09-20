import { useState } from 'react'
import { Panel, SectionHead, Button, Badge, Field, TextInput } from '../ui'
import { IconRoles, IconLock, IconLogout } from '../icons'
import { currentUser } from '../adminData'

export default function Profile({
  activeRole,
  onSwitchRole,
}: {
  activeRole: string
  onSwitchRole: (role: string) => void
}) {
  const { firstName, lastName, username, email, department, registeredAt, roles } = currentUser
  const initials = `${firstName[0] ?? ''}${lastName[0] ?? ''}`
  const multiRole = roles.length > 1

  return (
    <div>
      <SectionHead
        title="Hesap Ayarları"
        description="Kişisel bilgilerinizi, rollerinizi ve güvenlik ayarlarınızı buradan yönetebilirsiniz."
      />

      <div className="grid grid-cols-1 gap-4 lg:grid-cols-[340px_1fr]">
        {/* Profil kartı */}
        <Panel className="h-fit p-6">
          <div className="flex flex-col items-center text-center">
            <span className="flex h-24 w-24 items-center justify-center rounded-full bg-gradient-to-br from-navy-600 to-teal-500 text-2xl font-bold text-white shadow-lg shadow-navy-900/20 ring-4 ring-white">
              {initials}
            </span>
            <h3 className="mt-4 text-lg font-bold text-navy-900">
              {firstName} {lastName}
            </h3>
            <p className="text-sm text-muted-foreground">{email}</p>
            <div className="mt-3 flex flex-wrap justify-center gap-1.5">
              {roles.map((r) => (
                <Badge key={r} tone={r === activeRole ? 'active' : 'admin'}>
                  {r}
                  {r === activeRole && ' • aktif'}
                </Badge>
              ))}
            </div>
          </div>

          <dl className="mt-6 divide-y divide-navy-50 text-sm">
            <Row label="Kullanıcı Adı" value={username} />
            <Row label="Departman" value={department} />
            <Row label="Aktif Rol" value={activeRole} />
            <Row label="Kayıt Tarihi" value={registeredAt} />
          </dl>
        </Panel>

        <div className="space-y-4">
          {/* Rol seçimi — yalnızca birden fazla rol varsa */}
          {multiRole && (
            <Panel className="p-6">
              <div className="flex items-center gap-2">
                <IconRoles className="h-5 w-5 text-teal-600" />
                <h3 className="text-base font-bold text-navy-900">Rollerim</h3>
              </div>
              <p className="mt-1 text-sm text-muted-foreground">
                Birden fazla rolünüz bulunuyor. Sisteme hangi rol ile devam etmek istediğinizi seçebilirsiniz.
              </p>
              <ul className="mt-4 space-y-2">
                {roles.map((r) => {
                  const isActive = r === activeRole
                  return (
                    <li
                      key={r}
                      className={`flex items-center gap-3 rounded-xl border px-4 py-3 transition-colors ${
                        isActive ? 'border-teal-300 bg-teal-50/60' : 'border-navy-100 hover:bg-navy-50'
                      }`}
                    >
                      <span
                        className={`flex h-9 w-9 items-center justify-center rounded-lg ${
                          isActive ? 'bg-teal-500 text-white' : 'bg-navy-100 text-navy-600'
                        }`}
                      >
                        <IconRoles className="h-5 w-5" />
                      </span>
                      <div className="min-w-0">
                        <p className="font-semibold text-navy-900">{r}</p>
                        <p className="text-xs text-muted-foreground">
                          {isActive ? 'Şu anda bu rol ile giriş yaptınız.' : 'Bu rol ile sisteme giriş yapabilirsiniz.'}
                        </p>
                      </div>
                      <div className="ml-auto">
                        {isActive ? (
                          <Badge tone="active">Aktif Rol</Badge>
                        ) : (
                          <Button variant="soft" onClick={() => onSwitchRole(r)}>
                            <IconLogout className="h-4 w-4" />
                            Bu rol ile gir
                          </Button>
                        )}
                      </div>
                    </li>
                  )
                })}
              </ul>
            </Panel>
          )}

          {/* Güvenlik ayarları */}
          <Panel className="p-6">
            <h3 className="text-base font-bold text-navy-900">Güvenlik Ayarları</h3>
            <div className="mt-4 flex items-start gap-3 rounded-xl bg-teal-50 p-4 text-sm text-teal-800">
              <IconLock className="mt-0.5 h-5 w-5 shrink-0 text-teal-600" />
              <p>
                Hesabınızın güvenliğini sağlamak için şifrenizi düzenli olarak değiştirmeniz önerilir. Yeni
                şifrenizin karmaşık ve tahmin edilemez olmasına dikkat edin.
              </p>
            </div>

            <div className="mt-5 space-y-4">
              <Field label="Mevcut Şifre">
                <TextInput type="password" placeholder="••••••••" />
              </Field>
              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <Field label="Yeni Şifre">
                  <TextInput type="password" placeholder="••••••••" />
                </Field>
                <Field label="Yeni Şifre (Tekrar)">
                  <TextInput type="password" placeholder="••••••••" />
                </Field>
              </div>
            </div>
            <div className="mt-5 flex justify-end">
              <Button>Şifreyi Güncelle</Button>
            </div>
          </Panel>
        </div>
      </div>
    </div>
  )
}

function Row({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex items-center justify-between py-3">
      <dt className="text-muted-foreground">{label}</dt>
      <dd className="font-semibold text-navy-900">{value}</dd>
    </div>
  )
}
