export type User = {
  id: number
  username: string
  email: string
  firstName: string | null
  lastName: string | null
  phone: string | null
  departmentId: number | null
  departmentName: string | null
  roles: string[]
  roleIds: number[]
  activeRoleId: number | null
  activeRoleName: string | null
  isActive: boolean
  createdDate: string
}

export type AuthResult = {
  accessToken: string
  refreshToken: string
  expiryDate: string
  user: User
}

export type Role = {
  id: number
  name: string
  description: string | null
  permissions: Permission[]
  isPrivileged: boolean
  isSystemRole: boolean
  isActive: boolean
  createdDate: string
}

// ASP.NET Core serializes the current CRUDActionType enum as its numeric value.
export type CrudActionType = 1 | 2 | 3 | 4 | 5 | 6

export type Permission = {
  id: number
  name: string
  code: string
  description: string | null
  controllerName: string
  actionName: string
  crudActionType: CrudActionType
  isActive: boolean
}

export type Department = {
  id: number
  name: string
  code: string
  parentDepartmentId: number | null
  parentDepartmentName: string | null
  subDepartments: Department[]
  isActive: boolean
}

export type Menu = {
  id: number
  name: string
  url: string | null
  icon: string | null
  parentId: number | null
  subMenus: Menu[]
  order: number
  permissionId: number | null
  permissionCode: string | null
  isActive: boolean
}

export type Setting = { id: number; key: string; value: string; description: string }
export type AuditLog = {
  id: number
  userId: number | null
  username: string | null
  action: string
  controller: string
  route: string
  details: string | null
  createdDate: string
}
export type RecycleBinItem = {
  id: number
  type: string
  typeName: string
  name: string
  deletedDate: string | null
}
export type DashboardStats = {
  totalUsers: number
  activeUsers: number
  totalRoles: number
  totalDepartments: number
  roleDistributions: { roleName: string; userCount: number }[]
}

export type CreateUserInput = {
  username: string
  email: string
  password: string
  firstName?: string
  lastName?: string
  phone?: string
  departmentId: number | null
  roleIds: number[]
}
export type UpdateUserInput = Omit<CreateUserInput, 'roleIds'> & { isActive: boolean }
export type RoleInput = { name: string; description: string; isActive: boolean; permissionIds: number[] }
export type PermissionInput = {
  name: string
  code: string
  description: string
  controllerName: string
  actionName: string
  crudActionType: CrudActionType
  isActive: boolean
}
export type DepartmentInput = { name: string; code: string; parentDepartmentId: number | null; isActive: boolean }
export type MenuInput = {
  name: string
  url: string
  icon: string
  parentId: number | null
  order: number
  permissionId: number | null
  isActive: boolean
}
