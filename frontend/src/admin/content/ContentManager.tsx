import { useMemo, useState } from 'react'
import {
  Panel,
  SectionHead,
  Button,
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
import type { ContentConfig, FieldDef, Row } from './contentConfig'

/** Config ile sürülen jenerik içerik yönetim ekranı (liste + ekle/düzenle/sil). */
export default function ContentManager({ config }: { config: ContentConfig }) {
  const [rows, setRows] = useState<Row[]>(config.rows)
  const [query, setQuery] = useState('')
  const [editing, setEditing] = useState<Row | 'new' | null>(null)

  // Config değiştiğinde (başka bir menüye geçildiğinde) veriyi sıfırla.
  const [configRef, setConfigRef] = useState(config)
  if (configRef !== config) {
    setConfigRef(config)
    setRows(config.rows)
    setQuery('')
    setEditing(null)
  }

  const filtered = useMemo(() => {
    const q = query.trim().toLowerCase()
    if (!q) return rows
    return rows.filter((r) =>
      Object.values(r).some((v) => String(v).toLowerCase().includes(q)),
    )
  }, [rows, query])

  function remove(id: number) {
    setRows((prev) => prev.filter((r) => r.id !== id))
  }

  function save(draft: Row, id?: number) {
    if (id) {
      setRows((prev) => prev.map((r) => (r.id === id ? { ...r, ...draft } : r)))
    } else {
      setRows((prev) => [...prev, { ...draft, id: Math.max(0, ...prev.map((r) => r.id)) + 1 }])
    }
    setEditing(null)
  }

  return (
    <div>
      <SectionHead
        title={config.title}
        description={config.description}
        action={
          <Button onClick={() => setEditing('new')}>
            <IconPlus className="h-4 w-4" /> {config.addLabel}
          </Button>
        }
      />

      <Panel className="overflow-hidden">
        <div className="border-b border-navy-50 px-5 py-4">
          <SearchInput value={query} onChange={setQuery} />
        </div>
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-navy-50 text-left text-xs font-semibold uppercase tracking-wide text-navy-400">
                {config.columns.map((c) => (
                  <th key={c.key} className="px-5 py-3">
                    {c.label}
                  </th>
                ))}
                <th className="px-5 py-3 text-right">İşlemler</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-navy-50">
              {filtered.map((row) => (
                <tr key={row.id} className="transition-colors hover:bg-navy-50/50">
                  {config.columns.map((c, i) => (
                    <td
                      key={c.key}
                      className={`px-5 py-3.5 ${i === 0 ? 'font-medium text-navy-900' : 'text-navy-600'}`}
                    >
                      {c.render ? c.render(row[c.key], row) : String(row[c.key] ?? '—')}
                    </td>
                  ))}
                  <td className="px-5 py-3.5">
                    <div className="flex justify-end gap-2">
                      <IconButton tone="edit" aria-label="Düzenle" onClick={() => setEditing(row)}>
                        <IconEdit className="h-4 w-4" />
                      </IconButton>
                      <IconButton tone="delete" aria-label="Sil" onClick={() => remove(row.id)}>
                        <IconTrash className="h-4 w-4" />
                      </IconButton>
                    </div>
                  </td>
                </tr>
              ))}
              {filtered.length === 0 && (
                <tr>
                  <td colSpan={config.columns.length + 1} className="px-5 py-10 text-center text-muted-foreground">
                    Kayıt bulunamadı.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </Panel>

      {editing && (
        <ContentModal
          config={config}
          row={editing === 'new' ? null : editing}
          onClose={() => setEditing(null)}
          onSave={save}
        />
      )}
    </div>
  )
}

function ContentModal({
  config,
  row,
  onClose,
  onSave,
}: {
  config: ContentConfig
  row: Row | null
  onClose: () => void
  onSave: (draft: Row, id?: number) => void
}) {
  const [draft, setDraft] = useState<Row>(() => {
    if (row) return { ...row }
    // Yeni kayıt için alanlardan boş taslak üret.
    const base: Row = { id: 0 }
    config.fields.forEach((f) => {
      base[f.key] = f.type === 'checkbox' ? true : f.type === 'number' ? 0 : ''
    })
    return base
  })
  const set = (key: string, value: unknown) => setDraft((d) => ({ ...d, [key]: value }))

  const imageField = config.fields.find((f) => f.type === 'image')

  return (
    <Modal
      title={row ? 'Kaydı Düzenle' : config.addLabel}
      onClose={onClose}
      footer={
        <>
          <Button variant="soft" onClick={onClose}>
            İptal
          </Button>
          <Button onClick={() => onSave(draft, row?.id)}>Kaydet</Button>
        </>
      }
    >
      {imageField && draft[imageField.key] ? (
        <div className="mb-4 overflow-hidden rounded-xl border border-navy-100">
          <img src={String(draft[imageField.key])} alt="" className="h-40 w-full object-cover" />
        </div>
      ) : null}

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        {config.fields.map((f) => (
          <div key={f.key} className={f.full || f.type === 'textarea' ? 'sm:col-span-2' : ''}>
            <FieldInput field={f} value={draft[f.key]} onChange={(v) => set(f.key, v)} />
          </div>
        ))}
      </div>
    </Modal>
  )
}

function FieldInput({
  field,
  value,
  onChange,
}: {
  field: FieldDef
  value: unknown
  onChange: (v: unknown) => void
}) {
  if (field.type === 'checkbox') {
    return <Check checked={Boolean(value)} onChange={onChange} label={field.label} />
  }
  if (field.type === 'textarea') {
    return (
      <Field label={field.label} hint={field.hint}>
        <Textarea value={String(value ?? '')} onChange={(e) => onChange(e.target.value)} />
      </Field>
    )
  }
  if (field.type === 'select') {
    return (
      <Field label={field.label} hint={field.hint}>
        <Select value={String(value ?? '')} onChange={(e) => onChange(e.target.value)}>
          <option value="">Seçiniz</option>
          {field.options?.map((o) => (
            <option key={o} value={o}>
              {o}
            </option>
          ))}
        </Select>
      </Field>
    )
  }
  const inputType = field.type === 'number' ? 'number' : field.type === 'date' ? 'date' : 'text'
  return (
    <Field label={field.label} hint={field.hint}>
      <TextInput
        type={inputType}
        value={String(value ?? '')}
        onChange={(e) => onChange(field.type === 'number' ? Number(e.target.value) : e.target.value)}
        className={field.type === 'image' ? 'font-mono text-xs' : ''}
        placeholder={field.type === 'image' ? 'https://...' : ''}
      />
    </Field>
  )
}
