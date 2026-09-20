import type {
  AuthResult,
  DashboardStats,
  Department,
  DepartmentInput,
  Menu,
  MenuInput,
  Permission,
  PermissionInput,
  RecycleBinItem,
  Role,
  RoleInput,
  Setting,
  UpdateUserInput,
  User,
  CreateUserInput,
  AuditLog,
} from './contracts'

const baseUrl = (import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5298').replace(/\/$/, '')
const storageKey = 'baselib.auth'

type ApiEnvelope<T> = { success?: boolean; message?: string; data?: T }
type StoredSession = Pick<AuthResult, 'accessToken' | 'refreshToken' | 'expiryDate' | 'user'>

export class ApiError extends Error {
  constructor(message: string, public readonly status: number) {
    super(message)
  }
}

let refreshing: Promise<AuthResult | null> | null = null

export function getSession(): StoredSession | null {
  const raw = localStorage.getItem(storageKey)
  if (!raw) return null
  try {
    return JSON.parse(raw) as StoredSession
  } catch {
    localStorage.removeItem(storageKey)
    return null
  }
}

export function saveSession(session: AuthResult) {
  localStorage.setItem(storageKey, JSON.stringify(session))
}

export function clearSession() {
  localStorage.removeItem(storageKey)
}

async function request<T>(path: string, init: RequestInit = {}, retry = true): Promise<T> {
  const session = getSession()
  const response = await fetch(`${baseUrl}${path}`, {
    ...init,
    headers: {
      Accept: 'application/json',
      ...(init.body ? { 'Content-Type': 'application/json' } : {}),
      ...(session ? { Authorization: `Bearer ${session.accessToken}` } : {}),
      ...init.headers,
    },
  })
  const text = await response.text()
  let payload: ApiEnvelope<T> | T | null = null
  try {
    payload = text ? (JSON.parse(text) as ApiEnvelope<T> | T) : null
  } catch {
    throw new ApiError('Sunucudan geçersiz bir yanıt alındı.', response.status)
  }

  if (response.status === 401 && retry && session?.refreshToken && path !== '/api/auth/refresh') {
    const renewed = await refreshSession()
    if (renewed) return request<T>(path, init, false)
  }

  const envelope = payload as ApiEnvelope<T> | null
  if (!response.ok || envelope?.success === false) {
    throw new ApiError(envelope?.message ?? 'İşlem tamamlanamadı.', response.status)
  }
  return envelope && 'data' in envelope ? (envelope.data as T) : (payload as T)
}

async function refreshSession(): Promise<AuthResult | null> {
  if (refreshing) return refreshing
  const session = getSession()
  if (!session?.refreshToken) return null
  refreshing = request<AuthResult>('/api/auth/refresh', {
    method: 'POST',
    body: JSON.stringify({ refreshToken: session.refreshToken }),
  }, false)
    .then((result) => {
      saveSession(result)
      return result
    })
    .catch(() => {
      clearSession()
      return null
    })
    .finally(() => {
      refreshing = null
    })
  return refreshing
}

const json = (body: unknown) => JSON.stringify(body)

export const api = {
  login: async (username: string, password: string) => {
    const result = await request<AuthResult>('/api/auth/login', { method: 'POST', body: json({ username, password }) })
    saveSession(result)
    return result
  },
  logout: async () => {
    try { await request<void>('/api/auth/logout', { method: 'POST' }) } finally { clearSession() }
  },
  switchRole: async (roleId: number) => {
    const result = await request<AuthResult>(`/api/auth/switch-role/${roleId}`, { method: 'POST' })
    saveSession(result)
    return result
  },
  profile: () => request<User>('/api/profile'),
  changePassword: (currentPassword: string, newPassword: string) =>
    request<void>('/api/profile/password', { method: 'PUT', body: json({ currentPassword, newPassword }) }),
  dashboard: () => request<DashboardStats>('/api/dashboard/stats'),
  users: () => request<User[]>('/api/users'),
  createUser: (body: CreateUserInput) => request<User>('/api/users', { method: 'POST', body: json(body) }),
  updateUser: (id: number, body: UpdateUserInput) => request<void>(`/api/users/${id}`, { method: 'PUT', body: json(body) }),
  deleteUser: (id: number) => request<void>(`/api/users/${id}`, { method: 'DELETE' }),
  assignUserRoles: (id: number, roleIds: number[]) => request<void>(`/api/users/${id}/roles`, { method: 'PUT', body: json(roleIds) }),
  roles: () => request<Role[]>('/api/roles'),
  createRole: (body: RoleInput) => request<Role>('/api/roles', { method: 'POST', body: json(body) }),
  updateRole: (id: number, body: RoleInput) => request<void>(`/api/roles/${id}`, { method: 'PUT', body: json(body) }),
  deleteRole: (id: number) => request<void>(`/api/roles/${id}`, { method: 'DELETE' }),
  permissions: () => request<Permission[]>('/api/permissions'),
  createPermission: (body: PermissionInput) => request<Permission>('/api/permissions', { method: 'POST', body: json(body) }),
  updatePermission: (id: number, body: PermissionInput) => request<void>(`/api/permissions/${id}`, { method: 'PUT', body: json(body) }),
  deletePermission: (id: number) => request<void>(`/api/permissions/${id}`, { method: 'DELETE' }),
  departments: () => request<Department[]>('/api/departments'),
  createDepartment: (body: Omit<DepartmentInput, 'isActive'>) => request<Department>('/api/departments', { method: 'POST', body: json(body) }),
  updateDepartment: (id: number, body: DepartmentInput) => request<void>(`/api/departments/${id}`, { method: 'PUT', body: json(body) }),
  deleteDepartment: (id: number) => request<void>(`/api/departments/${id}`, { method: 'DELETE' }),
  menus: () => request<Menu[]>('/api/menus'),
  myMenus: () => request<Menu[]>('/api/menus/me'),
  createMenu: (body: Omit<MenuInput, 'isActive'>) => request<Menu>('/api/menus', { method: 'POST', body: json(body) }),
  updateMenu: (id: number, body: MenuInput) => request<void>(`/api/menus/${id}`, { method: 'PUT', body: json(body) }),
  deleteMenu: (id: number) => request<void>(`/api/menus/${id}`, { method: 'DELETE' }),
  settings: () => request<Setting[]>('/api/settings'),
  updateSetting: (id: number, value: string) => request<void>(`/api/settings/${id}`, { method: 'PUT', body: json({ value }) }),
  auditLogs: () => request<AuditLog[]>('/api/auditlogs'),
  recycleBin: () => request<RecycleBinItem[]>('/api/recyclebin'),
  restoreRecycleBinItem: (type: string, id: number) => request<void>(`/api/recyclebin/${encodeURIComponent(type)}/${id}/restore`, { method: 'PUT' }),
}
