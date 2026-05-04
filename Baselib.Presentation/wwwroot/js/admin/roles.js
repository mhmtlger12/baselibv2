let roles = [];
let basePermissionGroups = [];
let activePermissionGroups = [];
let modal;

document.addEventListener('DOMContentLoaded', async () => {
    modal = new bootstrap.Modal(document.getElementById('roleModal'));
    document.getElementById('openRoleModal')?.addEventListener('click', openModal);
    document.getElementById('saveRoleButton')?.addEventListener('click', saveRole);
    document.querySelector('#roleTable tbody')?.addEventListener('click', handleTableAction);
    await loadData();
});

async function loadData() {
    try {
        showLoading();
        const [rolesRes, permissionGroupsRes] = await Promise.all([
            api.get('/api/roles'),
            api.get('/api/permissions/grouped')
        ]);

        roles = rolesRes || [];
        basePermissionGroups = permissionGroupsRes || [];
        renderTable();
    } catch (error) {
        showToast(error.message || 'Veriler yüklenirken hata oluştu', 'error');
    } finally {
        hideLoading();
    }
}

function renderTable() {
    const tbody = document.querySelector('#roleTable tbody');
    if (!tbody) return;

    tbody.innerHTML = roles.map(role => `
        <tr>
            <td>${escapeHtml(role.name)}</td>
            <td>${escapeHtml(role.description || '-')}</td>
            <td>
                <span class="badge ${role.isActive ? 'bg-success' : 'bg-secondary'}">
                    ${role.isActive ? 'Aktif' : 'Pasif'}
                </span>
            </td>
            <td>${role.permissionCount ?? role.permissions?.length ?? 0}</td>
            <td class="table-actions">
                <button class="btn btn-sm btn-outline-primary" type="button" data-action="edit" data-id="${role.id}" title="Düzenle">
                    <i class="bi bi-pencil"></i>
                </button>
                <button class="btn btn-sm btn-outline-danger" type="button" data-action="delete" data-id="${role.id}" title="Sil">
                    <i class="bi bi-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

async function handleTableAction(event) {
    const button = event.target.closest('[data-action]');
    if (!button) return;

    const id = Number.parseInt(button.dataset.id, 10);
    if (button.dataset.action === 'edit') {
        await editRole(id);
        return;
    }

    if (button.dataset.action === 'delete')
        await deleteRole(id);
}

function renderPermissionGroups() {
    const container = document.getElementById('permissionsList');
    if (!container) return;

    if (!activePermissionGroups.length) {
        container.innerHTML = '<p class="text-muted mb-0">Tanımlı izin bulunamadı.</p>';
        return;
    }

    const middleIndex = Math.ceil(activePermissionGroups.length / 2);
    const columns = [
        activePermissionGroups.slice(0, middleIndex),
        activePermissionGroups.slice(middleIndex)
    ];

    container.innerHTML = `
        <div class="role-permission-grid">
            ${columns.map(groups => `<div class="role-permission-column">${groups.map(renderPermissionGroup).join('')}</div>`).join('')}
        </div>
    `;

    container.querySelectorAll('.controller-check').forEach(input => {
        input.addEventListener('change', event => {
            setControllerChecked(event.target.dataset.controller, event.target.checked);
        });
    });

    container.querySelectorAll('.perm-check').forEach(input => {
        input.addEventListener('change', event => {
            setPermissionChecked(Number.parseInt(event.target.value, 10), event.target.checked);
        });
    });

    refreshControllerStates();
}

function renderPermissionGroup(group) {
    const controllerKey = escapeAttribute(group.controllerName);

    return `
        <section class="role-permission-card">
            <div class="role-permission-header">
                <label class="form-check">
                    <input class="form-check-input controller-check"
                           type="checkbox"
                           data-controller="${controllerKey}">
                    <span class="form-check-label">${escapeHtml(group.controllerName)}</span>
                </label>
            </div>
            <div class="role-permission-actions">
                ${group.controllerCrudList.map(permission => renderPermissionAction(group.controllerName, permission)).join('')}
            </div>
        </section>
    `;
}

function renderPermissionAction(controllerName, permission) {
    const label = permission.name || permission.actionName || permission.code || permission.crudActionType;
    const title = [permission.actionName, permission.code].filter(Boolean).join(' - ');

    return `
        <label class="role-permission-action" title="${escapeAttribute(title)}">
            <input class="form-check-input perm-check"
                   type="checkbox"
                   data-controller="${escapeAttribute(controllerName)}"
                   value="${permission.permissionId}"
                   ${permission.checked ? 'checked' : ''}>
            <span>${escapeHtml(label)}</span>
        </label>
    `;
}

function openModal() {
    document.getElementById('modalTitle').textContent = 'Yeni Rol';
    document.getElementById('roleId').value = '';
    document.getElementById('name').value = '';
    document.getElementById('description').value = '';
    document.getElementById('isActive').checked = true;

    activePermissionGroups = cloneGroups(basePermissionGroups);
    renderPermissionGroups();
    modal.show();
}

async function editRole(id) {
    const role = roles.find(item => item.id === id);
    if (!role) return;

    try {
        showLoading();
        const groups = await api.get(`/api/roles/${id}/permissions`);

        document.getElementById('modalTitle').textContent = 'Rol Düzenle';
        document.getElementById('roleId').value = role.id;
        document.getElementById('name').value = role.name || '';
        document.getElementById('description').value = role.description || '';
        document.getElementById('isActive').checked = role.isActive ?? true;

        activePermissionGroups = cloneGroups(groups || []);
        renderPermissionGroups();
        modal.show();
    } catch (error) {
        showToast(error.message || 'Rol izinleri yüklenemedi', 'error');
    } finally {
        hideLoading();
    }
}

function setControllerChecked(controllerName, checked) {
    activePermissionGroups
        .filter(group => group.controllerName === controllerName)
        .forEach(group => {
            group.checked = checked;
            group.indeterminate = false;
            group.controllerCrudList.forEach(permission => {
                permission.checked = checked;
            });
        });

    renderPermissionGroups();
}

function setPermissionChecked(permissionId, checked) {
    for (const group of activePermissionGroups) {
        const permission = group.controllerCrudList.find(item => item.permissionId === permissionId);
        if (!permission) continue;

        permission.checked = checked;
        const checkedCount = group.controllerCrudList.filter(item => item.checked).length;
        group.checked = checkedCount > 0;
        group.indeterminate = checkedCount > 0 && checkedCount < group.controllerCrudList.length;
        break;
    }

    refreshControllerStates();
}

function refreshControllerStates() {
    document.querySelectorAll('.controller-check').forEach(input => {
        const group = activePermissionGroups.find(item => item.controllerName === input.dataset.controller);
        if (!group) return;

        const checkedCount = group.controllerCrudList.filter(item => item.checked).length;
        input.checked = checkedCount > 0;
        input.indeterminate = checkedCount > 0 && checkedCount < group.controllerCrudList.length;
    });
}

function collectPermissionIds() {
    return activePermissionGroups
        .flatMap(group => group.controllerCrudList)
        .filter(permission => permission.checked && permission.permissionId > 0)
        .map(permission => permission.permissionId);
}

async function saveRole() {
    const id = document.getElementById('roleId').value;
    const data = {
        name: document.getElementById('name').value.trim(),
        description: document.getElementById('description').value.trim(),
        isActive: document.getElementById('isActive').checked,
        permissionIds: collectPermissionIds()
    };

    if (!data.name) {
        showToast('Rol adı zorunludur', 'error');
        return;
    }

    try {
        showLoading();
        if (id) {
            await api.put(`/api/roles/${id}`, data);
        } else {
            await api.post('/api/roles', data);
        }

        modal.hide();
        await loadData();
        showToast('Rol kaydedildi');
    } catch (error) {
        showToast(error.message || 'Kayıt başarısız', 'error');
    } finally {
        hideLoading();
    }
}

async function deleteRole(id) {
    if (!confirmDelete()) return;

    try {
        showLoading();
        await api.delete(`/api/roles/${id}`);
        await loadData();
        showToast('Rol silindi');
    } catch (error) {
        showToast(error.message || 'Silme başarısız', 'error');
    } finally {
        hideLoading();
    }
}

function cloneGroups(groups) {
    return JSON.parse(JSON.stringify(groups || []));
}

function escapeAttribute(value) {
    return escapeHtml(value).replaceAll('"', '&quot;');
}
