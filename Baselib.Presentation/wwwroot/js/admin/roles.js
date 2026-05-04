// Roles Page JavaScript
let roles = [];
let permissions = [];
let rolePermissions = {};
let modal;

document.addEventListener('DOMContentLoaded', async function() {
    modal = new bootstrap.Modal(document.getElementById('roleModal'));
    await loadData();
});

async function loadData() {
    try {
        showLoading();
        const [rolesRes, permsRes] = await Promise.all([
            api.get('/api/roles'),
            api.get('/api/permissions')
        ]);
        
        roles = rolesRes || [];
        permissions = permsRes || [];
        
        renderTable();
        renderPermissionGroups();
    } catch (e) {
        showToast('Veriler yüklenirken hata oluştu', 'error');
    } finally {
        hideLoading();
    }
}

function renderTable() {
    const tbody = document.querySelector('#roleTable tbody');
    if (!tbody) return;
    
    tbody.innerHTML = roles.map(r => `
        <tr>
            <td>${escapeHtml(r.name)}</td>
            <td>${escapeHtml(r.description || '-')}</td>
            <td>
                <span class="badge ${r.isActive ? 'bg-success' : 'bg-secondary'}">
                    ${r.isActive ? 'Aktif' : 'Pasif'}
                </span>
            </td>
            <td>${r.permissionCount || 0}</td>
            <td>
                <button class="btn btn-sm btn-warning" onclick="editRole(${r.id})" title="Düzenle">
                    <i class="bi bi-pencil"></i>
                </button>
                <button class="btn btn-sm btn-danger" onclick="deleteRole(${r.id})" title="Sil">
                    <i class="bi bi-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

function renderPermissionGroups() {
    const container = document.getElementById('permissionsList');
    if (!container) return;
    
    const grouped = {};
    permissions.forEach(p => {
        const ctrl = p.controllerName || 'Diğer';
        if (!grouped[ctrl]) grouped[ctrl] = [];
        grouped[ctrl].push(p);
    });

    const controllers = Object.keys(grouped).sort();
    container.innerHTML = controllers.map(ctrl => `
        <div class="permission-group">
            <div class="group-header">
                <span>${ctrl}</span>
                <div class="group-actions">
                    <input type="checkbox" class="select-all" onchange="toggleGroup('${ctrl}', this.checked)">
                    <label>Tümü</label>
                </div>
            </div>
            <div class="group-items">
                ${grouped[ctrl].map(p => `
                    <div class="permission-item">
                        <input type="checkbox" class="perm-check" 
                               data-controller="${ctrl}" 
                               data-id="${p.id}" 
                               data-crud="${p.cRUDValue || p.crudValue || 0}"
                               ${isPermissionSelected(ctrl, p.id) ? 'checked' : ''}>
                        <label>${escapeHtml(p.actionName || p.name)}</label>
                        <div class="crud-flags">
                            ${(p.cRUDValue || p.crudValue || 0) & 1 ? '<span class="crud-flag crud-c">C</span>' : ''}
                            ${(p.cRUDValue || p.crudValue || 0) & 2 ? '<span class="crud-flag crud-r">R</span>' : ''}
                            ${(p.cRUDValue || p.crudValue || 0) & 4 ? '<span class="crud-flag crud-u">U</span>' : ''}
                            ${(p.cRUDValue || p.crudValue || 0) & 8 ? '<span class="crud-flag crud-d">D</span>' : ''}
                        </div>
                    </div>
                `).join('')}
            </div>
        </div>
    `).join('');
}

function isPermissionSelected(controller, permId) {
    return rolePermissions[controller]?.includes(permId);
}

function openModal() {
    document.getElementById('modalTitle').textContent = 'Yeni Rol';
    document.getElementById('roleId').value = '';
    document.getElementById('name').value = '';
    document.getElementById('description').value = '';
    document.getElementById('isActive').checked = true;
    rolePermissions = {};
    renderPermissionGroups();
    modal.show();
}

function editRole(id) {
    const role = roles.find(r => r.id === id);
    if (!role) return;

    document.getElementById('modalTitle').textContent = 'Rol Düzenle';
    document.getElementById('roleId').value = role.id;
    document.getElementById('name').value = role.name || '';
    document.getElementById('description').value = role.description || '';
    document.getElementById('isActive').checked = role.isActive ?? true;
    
    // Load role permissions
    rolePermissions = role.permissions || {};
    renderPermissionGroups();
    
    modal.show();
}

function toggleGroup(controller, checked) {
    document.querySelectorAll(`.perm-check[data-controller="${controller}"]`).forEach(cb => {
        cb.checked = checked;
    });
}

function collectPermissions() {
    const perms = {};
    document.querySelectorAll('.permission-group').forEach(group => {
        const controller = group.querySelector('.perm-check')?.dataset.controller;
        if (!controller) return;
        
        const selectedIds = [];
        let crudValue = 0;
        
        group.querySelectorAll('.perm-check:checked').forEach(cb => {
            selectedIds.push(parseInt(cb.dataset.id));
            crudValue |= parseInt(cb.dataset.crud || 0);
        });
        
        if (selectedIds.length > 0) {
            perms[controller] = { ids: selectedIds, crud: crudValue };
        }
    });
    return perms;
}

async function saveRole() {
    const id = document.getElementById('roleId').value;
    const data = {
        id: id ? parseInt(id) : 0,
        name: document.getElementById('name').value,
        description: document.getElementById('description').value,
        isActive: document.getElementById('isActive').checked,
        permissions: collectPermissions()
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
    } catch (e) {
        showToast('Kayıt başarısız', 'error');
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
    } catch (e) {
        showToast('Silme başarısız', 'error');
    } finally {
        hideLoading();
    }
}

function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

window.openModal = openModal;
window.editRole = editRole;
window.saveRole = saveRole;
window.deleteRole = deleteRole;
window.toggleGroup = toggleGroup;