let modal;
let permissions = [];
let permissionSearch = '';

const crudLabels = {
    1: 'View',
    2: 'Add',
    3: 'Update',
    4: 'Preview',
    5: 'Option',
    6: 'Delete'
};

document.addEventListener('DOMContentLoaded', async () => {
    modal = new bootstrap.Modal(document.getElementById('permModal'));
    document.getElementById('openPermModal')?.addEventListener('click', openModal);
    document.getElementById('savePermButton')?.addEventListener('click', savePerm);
    document.getElementById('permissionList')?.addEventListener('click', handleTableAction);
    document.getElementById('permissionList')?.addEventListener('input', handleSearch);
    await loadData();
});

async function loadData() {
    try {
        showLoading();
        permissions = await api.get('/api/permissions') || [];
        renderList();
    } catch (error) {
        showToast(error.message || 'İzinler yüklenemedi', 'error');
    } finally {
        hideLoading();
    }
}

function renderList() {
    const container = document.getElementById('permissionList');
    if (!container) return;

    const rows = filterPermissions(permissions);
    container.innerHTML = `
        <div class="table-toolbar">
            <button class="icon-button" type="button" data-action="refresh" title="Yenile">
                <i class="bi bi-arrow-clockwise"></i>
            </button>
            <div class="search-box">
                <i class="bi bi-search"></i>
                <input class="form-control" id="permissionSearch" type="search" value="${escapeAttribute(permissionSearch)}" placeholder="Ara" />
            </div>
        </div>
        <div class="table-responsive">
            <table class="table table-hover admin-table">
                <thead>
                    <tr>
                        <th>Adı</th>
                        <th>Controller Name</th>
                        <th>Action Name</th>
                        <th>Açıklama</th>
                        <th>Crud Action Type</th>
                        <th>Onay Durumu</th>
                        <th class="table-actions">Ayarlar</th>
                    </tr>
                </thead>
                <tbody>
                    ${rows.length ? rows.map(renderPermissionRow).join('') : '<tr><td colspan="7" class="text-muted">İzin bulunamadı.</td></tr>'}
                </tbody>
            </table>
        </div>
    `;
}

function renderPermissionRow(permission) {
    return `
        <tr>
            <td>${escapeHtml(permission.name)}</td>
            <td>${escapeHtml(permission.controllerName)}</td>
            <td>${escapeHtml(permission.actionName)}</td>
            <td>${escapeHtml(permission.description || '-')}</td>
            <td><span class="crud-pill">${escapeHtml(getCrudLabel(permission.crudActionType))}</span></td>
            <td>
                <span class="badge ${permission.isActive ? 'bg-success' : 'bg-secondary'}">
                    ${permission.isActive ? 'Aktif' : 'Pasif'}
                </span>
            </td>
            <td class="table-actions">
                <button class="btn btn-sm btn-outline-primary" type="button" data-action="edit" data-id="${permission.id}" title="Düzenle">
                    <i class="bi bi-pencil"></i>
                </button>
                <button class="btn btn-sm btn-outline-danger" type="button" data-action="delete" data-id="${permission.id}" title="Sil">
                    <i class="bi bi-trash"></i>
                </button>
            </td>
        </tr>
    `;
}

function handleTableAction(event) {
    const button = event.target.closest('[data-action]');
    if (!button) return;

    if (button.dataset.action === 'refresh') {
        loadData();
        return;
    }

    const id = Number.parseInt(button.dataset.id, 10);
    if (button.dataset.action === 'edit') {
        editPerm(id);
        return;
    }

    if (button.dataset.action === 'delete')
        deletePerm(id);
}

function handleSearch(event) {
    if (event.target.id !== 'permissionSearch') return;

    permissionSearch = event.target.value;
    renderList();
    document.getElementById('permissionSearch')?.focus();
}

function openModal() {
    document.getElementById('modalTitle').textContent = 'Yeni İzin';
    document.getElementById('permId').value = '';
    document.getElementById('name').value = '';
    document.getElementById('controllerName').value = '';
    document.getElementById('actionName').value = '';
    document.getElementById('description').value = '';
    document.getElementById('isActive').checked = true;
    document.querySelectorAll('.crud-type').forEach(item => item.checked = false);
    modal.show();
}

function editPerm(id) {
    const permission = permissions.find(item => item.id === id);
    if (!permission) return;

    document.getElementById('modalTitle').textContent = 'İzin Düzenle';
    document.getElementById('permId').value = permission.id;
    document.getElementById('name').value = permission.name || '';
    document.getElementById('controllerName').value = permission.controllerName || '';
    document.getElementById('actionName').value = permission.actionName || '';
    document.getElementById('description').value = permission.description || '';
    document.getElementById('isActive').checked = permission.isActive ?? true;

    document.querySelectorAll('.crud-type').forEach(item => {
        item.checked = Number.parseInt(item.value, 10) === (permission.crudActionType || 0);
    });

    modal.show();
}

async function savePerm() {
    const id = document.getElementById('permId').value;
    const selectedCrud = document.querySelector('.crud-type:checked');
    const controllerName = document.getElementById('controllerName').value.trim();
    const actionName = document.getElementById('actionName').value.trim();
    const data = {
        name: document.getElementById('name').value.trim(),
        code: `${controllerName}_${actionName}`,
        controllerName,
        actionName,
        crudActionType: selectedCrud ? Number.parseInt(selectedCrud.value, 10) : 0,
        description: document.getElementById('description').value.trim(),
        isActive: document.getElementById('isActive').checked
    };

    if (!data.name || !data.controllerName || !data.actionName || !data.crudActionType) {
        showToast('İzin adı, controller, action ve CRUD tipi zorunludur', 'error');
        return;
    }

    try {
        showLoading();
        if (id) {
            await api.put(`/api/permissions/${id}`, data);
        } else {
            await api.post('/api/permissions', data);
        }

        modal.hide();
        await loadData();
        showToast('İzin kaydedildi');
    } catch (error) {
        showToast(error.message || 'Kayıt başarısız', 'error');
    } finally {
        hideLoading();
    }
}

async function deletePerm(id) {
    if (!confirmDelete()) return;

    try {
        showLoading();
        await api.delete(`/api/permissions/${id}`);
        await loadData();
        showToast('İzin silindi');
    } catch (error) {
        showToast(error.message || 'Silme başarısız', 'error');
    } finally {
        hideLoading();
    }
}

function filterPermissions(items) {
    const search = permissionSearch.trim().toLocaleLowerCase('tr-TR');
    if (!search) return items;

    return items.filter(permission => [
        permission.name,
        permission.controllerName,
        permission.actionName,
        permission.description,
        getCrudLabel(permission.crudActionType)
    ].some(value => String(value || '').toLocaleLowerCase('tr-TR').includes(search)));
}

function getCrudLabel(value) {
    return crudLabels[value] || value || '-';
}

function escapeAttribute(value) {
    return escapeHtml(value).replaceAll('"', '&quot;');
}
