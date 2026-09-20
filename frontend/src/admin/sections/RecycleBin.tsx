import { useState } from 'react'
import { Panel, SectionHead, Badge, Button, IconButton } from '../ui'
import { IconRestore, IconTrash } from '../icons'
import { recycleItems, type RecycleItem } from '../adminData'

export default function RecycleBin() {
  const [items, setItems] = useState<RecycleItem[]>(recycleItems)

  function restore(id: number) {
    setItems((prev) => prev.filter((i) => i.id !== id))
  }
  function purge(id: number) {
    setItems((prev) => prev.filter((i) => i.id !== id))
  }

  return (
    <div>
      <SectionHead
        title="Çöp Kutusu"
        description="Silinen kayıtları geri yükleyin veya kalıcı olarak temizleyin."
        action={
          items.length > 0 ? (
            <Button variant="danger" onClick={() => setItems([])}>
              <IconTrash className="h-4 w-4" /> Tümünü Temizle
            </Button>
          ) : undefined
        }
      />

      {items.length === 0 ? (
        <Panel className="flex flex-col items-center justify-center gap-3 py-16 text-center">
          <span className="flex h-14 w-14 items-center justify-center rounded-2xl bg-navy-50 text-navy-300">
            <IconTrash className="h-7 w-7" />
          </span>
          <p className="text-sm text-muted-foreground">Çöp kutusu boş.</p>
        </Panel>
      ) : (
        <Panel className="overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-navy-50 text-left text-xs font-semibold uppercase tracking-wide text-navy-400">
                  <th className="px-5 py-3">Tür</th>
                  <th className="px-5 py-3">Kayıt</th>
                  <th className="px-5 py-3">Silen</th>
                  <th className="px-5 py-3">Silinme Tarihi</th>
                  <th className="px-5 py-3 text-right">İşlemler</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-navy-50">
                {items.map((i) => (
                  <tr key={i.id} className="transition-colors hover:bg-navy-50/50">
                    <td className="px-5 py-3.5">
                      <Badge tone="admin">{i.entity}</Badge>
                    </td>
                    <td className="px-5 py-3.5 font-medium text-navy-900">{i.name}</td>
                    <td className="px-5 py-3.5 text-navy-600">{i.deletedBy}</td>
                    <td className="px-5 py-3.5 whitespace-nowrap font-mono text-xs text-navy-400">
                      {i.deletedAt}
                    </td>
                    <td className="px-5 py-3.5">
                      <div className="flex justify-end gap-2">
                        <IconButton tone="plain" aria-label="Geri yükle" onClick={() => restore(i.id)}>
                          <IconRestore className="h-4 w-4" />
                        </IconButton>
                        <IconButton tone="delete" aria-label="Kalıcı sil" onClick={() => purge(i.id)}>
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
      )}
    </div>
  )
}
