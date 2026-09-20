import { useRef, useState } from 'react'
import type { Comment } from '../data/site'

const MAX_DEPTH = 2

type ReplyFormProps = {
  onSubmit: (name: string, body: string) => void
  onCancel?: () => void
  compact?: boolean
}

function RichEditor({ value, onChange, compact }: { value: string; onChange: (value: string) => void; compact: boolean }) {
  const editor = useRef<HTMLDivElement>(null)
  const apply = (command: 'bold' | 'italic' | 'insertUnorderedList') => {
    editor.current?.focus()
    document.execCommand(command)
    onChange(editor.current?.innerHTML || '')
  }
  return (
    <div className="overflow-hidden rounded-xl border border-border bg-white focus-within:border-teal-500 focus-within:ring-2 focus-within:ring-teal-100">
      <div className="flex items-center gap-1 border-b border-border bg-navy-50/70 px-2 py-1.5" aria-label="Yazı düzenleme araçları">
        <button type="button" onClick={() => apply('bold')} aria-label="Kalın yaz" className="h-7 w-7 rounded text-sm font-bold text-navy-700 hover:bg-white">B</button>
        <button type="button" onClick={() => apply('italic')} aria-label="İtalik yaz" className="h-7 w-7 rounded text-sm italic text-navy-700 hover:bg-white">I</button>
        <button type="button" onClick={() => apply('insertUnorderedList')} aria-label="Madde işaretli liste" className="h-7 w-7 rounded text-navy-700 hover:bg-white">☷</button>
      </div>
      <div ref={editor} contentEditable role="textbox" aria-multiline="true" data-placeholder={compact ? 'Yanıtını yaz...' : 'Yorumunu yaz...'} onInput={(e) => onChange(e.currentTarget.innerHTML)} className={`editor-content px-3 py-2.5 text-sm text-navy-900 outline-none ${compact ? 'min-h-18' : 'min-h-28'}`} />
    </div>
  )
}

function ReplyForm({ onSubmit, onCancel, compact = false }: ReplyFormProps) {
  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [body, setBody] = useState('')
  const [hideName, setHideName] = useState(false)
  function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    if (!body.replace(/<[^>]*>/g, '').trim() || !name.trim() || !email.trim()) return
    onSubmit(hideName ? maskName(name.trim()) : name.trim(), body.trim())
    setName(''); setEmail(''); setBody('')
  }
  return (
    <form onSubmit={handleSubmit} className={compact ? 'mt-3' : 'mt-4 rounded-2xl border border-border bg-white p-4 sm:p-5'}>
      {!compact && <><div className="mb-4 flex items-center gap-3"><span className="flex h-9 w-9 items-center justify-center rounded-full bg-teal-100 font-semibold text-teal-700">✎</span><div><h3 className="font-semibold text-navy-900">Düşünceni paylaş</h3><p className="text-xs text-muted-foreground">Yapıcı yorumlar tercih sürecini kolaylaştırır.</p></div></div><div className="grid gap-3 sm:grid-cols-2"><div><label htmlFor="comment-name" className="mb-1.5 block text-sm font-medium text-navy-800">Adın soyadın</label><input required id="comment-name" value={name} onChange={(e) => setName(e.target.value)} placeholder="Örn. Ayşe Yılmaz" className="w-full rounded-lg border border-border px-3 py-2 text-sm outline-none focus:border-teal-500" /></div><div><label htmlFor="comment-email" className="mb-1.5 block text-sm font-medium text-navy-800">E-posta adresin</label><input required type="email" id="comment-email" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="ornek@mail.com" className="w-full rounded-lg border border-border px-3 py-2 text-sm outline-none focus:border-teal-500" /></div></div><label className="mt-3 flex cursor-pointer items-start gap-2 text-xs leading-5 text-muted-foreground"><input type="checkbox" checked={hideName} onChange={(e) => setHideName(e.target.checked)} className="mt-0.5 h-4 w-4 accent-teal-600" />Adım soyadım yorumda yıldızlanarak gizlensin.</label></>}
      {compact && <><label htmlFor="reply-name" className="sr-only">Adın soyadın</label><input required id="reply-name" value={name} onChange={(e) => setName(e.target.value)} placeholder="Adın soyadın" className="mb-2 w-full rounded-lg border border-border px-3 py-2 text-sm outline-none focus:border-teal-500" /><label htmlFor="reply-email" className="sr-only">E-posta adresin</label><input required type="email" id="reply-email" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="E-posta adresin" className="mb-2 w-full rounded-lg border border-border px-3 py-2 text-sm outline-none focus:border-teal-500" /></>}
      <div className="mt-3"><span className="mb-1.5 block text-sm font-medium text-navy-800">{compact ? 'Yanıtın' : 'Yorumun'}</span><RichEditor value={body} onChange={setBody} compact={compact} /></div>
      <div className="mt-3 flex items-center gap-2"><button type="submit" className="rounded-lg bg-navy-700 px-4 py-2 text-sm font-semibold text-white transition hover:bg-navy-800">{compact ? 'Yanıtla' : 'Yorumu gönder'}</button>{onCancel && <button type="button" onClick={onCancel} className="rounded-lg px-3 py-2 text-sm text-muted-foreground hover:text-navy-800">İptal</button>}</div>
    </form>
  )
}

function maskName(name: string) { return name.split(/\s+/).map((part) => `${part[0] || ''}${'*'.repeat(Math.max(2, part.length - 1))}`).join(' ') }

function CommentNode({ comment, depth, onReply }: { comment: Comment; depth: number; onReply: (parentId: number, name: string, body: string) => void }) {
  const [replying, setReplying] = useState(false); const [liked, setLiked] = useState(false); const canReply = depth < MAX_DEPTH
  return <li className="relative mt-4"><article className="rounded-xl border border-border bg-white p-4 shadow-sm shadow-navy-900/[0.02]"><header className="flex items-center gap-2"><span className="flex h-8 w-8 items-center justify-center rounded-full bg-navy-100 text-xs font-bold text-navy-700">{comment.author.charAt(0)}</span><div><span className="block text-sm font-semibold text-navy-800">{comment.author}</span><span className="text-xs text-muted-foreground">{comment.time}</span></div></header><div className="comment-body mt-3 text-sm leading-6 text-navy-900" dangerouslySetInnerHTML={{ __html: comment.body }} /><div className="mt-3 flex items-center gap-4 text-xs"><button onClick={() => setLiked((v) => !v)} aria-pressed={liked} className={liked ? 'font-semibold text-teal-700' : 'font-medium text-muted-foreground hover:text-navy-700'}>♥ Beğen {comment.likes + (liked ? 1 : 0)}</button>{canReply && <button onClick={() => setReplying((v) => !v)} className="font-medium text-muted-foreground hover:text-navy-700">↳ Yanıtla</button>}</div>{replying && canReply && <ReplyForm compact onSubmit={(name, body) => { onReply(comment.id, name, body); setReplying(false) }} onCancel={() => setReplying(false)} />}</article>{comment.replies.length > 0 && <ul className="ml-4 border-l-2 border-teal-200 pl-4 sm:ml-7 sm:pl-6">{comment.replies.map((reply) => <CommentNode key={reply.id} comment={reply} depth={depth + 1} onReply={onReply} />)}</ul>}</li>
}

let nextId = 1000
function addReply(list: Comment[], parentId: number, reply: Comment): Comment[] { return list.map((c) => c.id === parentId ? { ...c, replies: [...c.replies, reply] } : c.replies.length ? { ...c, replies: addReply(c.replies, parentId, reply) } : c) }
export default function CommentThread({ initial }: { initial: Comment[] }) {
  const [items, setItems] = useState(initial); const makeComment = (name: string, body: string): Comment => ({ id: ++nextId, author: name, time: 'şimdi', body, likes: 0, replies: [] }); const total = countComments(items)
  return <section aria-label="Yorumlar" className="mt-10"><div className="mb-4 flex items-end justify-between border-b border-border pb-3"><div><p className="text-xs font-semibold uppercase tracking-[0.16em] text-teal-700">Topluluk</p><h2 className="mt-1 text-xl font-bold text-navy-900">Yorumlar <span className="text-muted-foreground">({total})</span></h2></div><span className="hidden text-xs text-muted-foreground sm:block">Saygılı ve yapıcı olun.</span></div><ReplyForm onSubmit={(name, body) => setItems((prev) => [makeComment(name, body), ...prev])} /><ul className="mt-5">{items.map((c) => <CommentNode key={c.id} comment={c} depth={0} onReply={(parentId, name, body) => setItems((prev) => addReply(prev, parentId, makeComment(name, body)))} />)}</ul></section>
}
function countComments(list: Comment[]): number { return list.reduce((sum, c) => sum + 1 + countComments(c.replies), 0) }
