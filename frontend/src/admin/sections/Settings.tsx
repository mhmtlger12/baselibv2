import { useState } from 'react'
import { Panel, SectionHead, Button, Field, TextInput, Textarea, Select } from '../ui'

export default function Settings() {
  const [siteName, setSiteName] = useState('puannokta')
  const [tagline, setTagline] = useState('Taban puanları, tek noktada.')
  const [contactMail, setContactMail] = useState('iletisim@puannokta.com')
  const [description, setDescription] = useState(
    'KPSS, YKS, ALES ve DGS taban puanları ve başarı sıralamaları için güncel bilgi platformu.',
  )
  const [timezone, setTimezone] = useState('Europe/Istanbul')
  const [maintenance, setMaintenance] = useState(false)
  const [commentsRequireApproval, setCommentsRequireApproval] = useState(true)
  const [allowRegistration, setAllowRegistration] = useState(true)

  return (
    <div>
      <SectionHead
        title="Sistem Ayarları"
        description="Sitenin genel yapılandırmasını ve davranışını yönetin."
      />

      <div className="grid grid-cols-1 gap-4 xl:grid-cols-2">
        <Panel className="p-6">
          <h3 className="mb-4 text-sm font-bold text-navy-900">Genel</h3>
          <div className="space-y-4">
            <Field label="Site Adı">
              <TextInput value={siteName} onChange={(e) => setSiteName(e.target.value)} />
            </Field>
            <Field label="Slogan">
              <TextInput value={tagline} onChange={(e) => setTagline(e.target.value)} />
            </Field>
            <Field label="Site Açıklaması (SEO)">
              <Textarea value={description} onChange={(e) => setDescription(e.target.value)} />
            </Field>
          </div>
        </Panel>

        <Panel className="p-6">
          <h3 className="mb-4 text-sm font-bold text-navy-900">İletişim & Bölge</h3>
          <div className="space-y-4">
            <Field label="İletişim E-postası">
              <TextInput type="email" value={contactMail} onChange={(e) => setContactMail(e.target.value)} />
            </Field>
            <Field label="Saat Dilimi">
              <Select value={timezone} onChange={(e) => setTimezone(e.target.value)}>
                <option value="Europe/Istanbul">Europe/Istanbul (GMT+3)</option>
                <option value="Europe/London">Europe/London (GMT+0)</option>
                <option value="UTC">UTC</option>
              </Select>
            </Field>
          </div>
        </Panel>

        <Panel className="p-6 xl:col-span-2">
          <h3 className="mb-4 text-sm font-bold text-navy-900">Davranış</h3>
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
            <ToggleCard
              title="Bakım Modu"
              desc="Site ziyaretçilere kapatılır, yalnızca yöneticiler erişebilir."
              checked={maintenance}
              onChange={setMaintenance}
            />
            <ToggleCard
              title="Yorum Onayı"
              desc="Yeni yorumlar yayınlanmadan önce moderatör onayı bekler."
              checked={commentsRequireApproval}
              onChange={setCommentsRequireApproval}
            />
            <ToggleCard
              title="Üye Kaydı"
              desc="Ziyaretçilerin siteye kayıt olmasına izin verilir."
              checked={allowRegistration}
              onChange={setAllowRegistration}
            />
          </div>
        </Panel>
      </div>

      <div className="mt-4 flex justify-end">
        <Button>Değişiklikleri Kaydet</Button>
      </div>
    </div>
  )
}

function ToggleCard({
  title,
  desc,
  checked,
  onChange,
}: {
  title: string
  desc: string
  checked: boolean
  onChange: (v: boolean) => void
}) {
  return (
    <div className="rounded-xl border border-navy-100 p-4">
      <div className="flex items-center justify-between">
        <span className="font-semibold text-navy-900">{title}</span>
        <button
          onClick={() => onChange(!checked)}
          role="switch"
          aria-checked={checked}
          className={`relative h-6 w-11 rounded-full transition-colors ${checked ? 'bg-teal-500' : 'bg-navy-200'}`}
        >
          <span
            className={`absolute top-0.5 h-5 w-5 rounded-full bg-white shadow transition-all ${
              checked ? 'left-[22px]' : 'left-0.5'
            }`}
          />
        </button>
      </div>
      <p className="mt-2 text-xs text-muted-foreground">{desc}</p>
    </div>
  )
}
