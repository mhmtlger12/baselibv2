// Users Page JavaScript
let users = [];
let roles = [];
let departments = [];
let modal;

document.addEventListener('DOMContentLoaded', async function() {
    modal = new bootstrap.Modal(document.getElementById('userModal'));
    await loadData();
});

async function loadData() {
    try {
        showLoading();
        const [usersRes, rolesRes, deptRes] = await Promise.all([
            api.get('/api/users'),
            api.get('/api/roles'),
            api.get('/api/departments')
        ]);
        
        users = usersRes || [];
        roles = rolesRes || [];
        departments = deptRes || [];
        
        renderTable();
        renderDepartments();
        renderRoles();
    } catch (e) {
        showToast('Veriler yüklenirken hata oluştu', 'error');
    } finally {
        hideLoading();
    }
}

function renderTable() {
    const tbody = document.querySelector('#userTable tbody');
    if (!tbody) return;
    
    tbody.innerHTML = users.map(u => `
        <tr>
            <td>${escapeHtml(u.fullName || '')}</td>
            <td>${escapeHtml(u.email || '')}</td>
            <td>${escapeHtml(u.departmentName || '-')}</td>
            <td>${(u.roleNames || []).map(r => `<span class="badge bg-primary">${escapeHtml(r)}</span>`).join(' ')}</td>
            <td>
                <span class="badge ${u.isActive ? 'bg-success' : 'bg-danger'}">
                    ${u.isActive ? 'Aktif' : 'Pasif'}
                </span>
            </td>
            <td>
                <button class="btn btn-sm btn-warning" onclick="editUser(${u.id})" title="Düzenle">
                    <i class="bi bi-pencil"></i>
                </button>
                <button class="btn btn-sm btn-danger" onclick="deleteUser(${u.id})" title="Sil">
                    <i class="bi bi-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

function renderDepartments() {
    const select = document.getElementById('departmentId');
    if (!select) return;
    select.innerHTML = '<option value="">Seçiniz</option>' + 
        departments.map(d => `<option value="${d.id}">${escapeHtml(d.name)}</option>`).join('');
}

function renderRoles() {
    const container = document.getElementById('rolesList');
    if (!container) return;
    container.innerHTML = roles.map(r => `
        <div class="form-check">
            <input class="form-check-input role-checkbox" type="checkbox" value="${r.id}" id="role_${r.id}" />
            <label class="form-check-label" for="role_${r.id}">${escapeHtml(r.name)}</label>
        </div>
    `).join('');
}

function openModal() {
    document.getElementById('modalTitle').textContent = 'Yeni Kullanıcı';
    document.getElementById('userId').value = '';
    document.getElementById('fullName').value = '';
    document.getElementById('email').value = '';
    document.getElementById('password').value = '';
    document.getElementById('departmentId').value = '';
    document.getElementById('isActive').checked = true;
    document.querySelectorAll('.role-checkbox').forEach(c => c.checked = false);
    modal.show();
}

function editUser(id) {
    const user = users.find(u => u.id === id);
    if (!user) return;

    document.getElementById('modalTitle').textContent = 'Kullanıcı Düzenle';
    document.getElementById('userId').value = user.id;
    document.getElementById('fullName').value = user.fullName || '';
    document.getElementById('email').value = user.email || '';
    document.getElementById('password').value = '';
    document.getElementById('departmentId').value = user.departmentId || '';
    document.getElementById('isActive').checked = user.isActive ?? true;
    
    document.querySelectorAll('.role-checkbox').forEach(c => {
        c.checked = user.roleIds?.includes(parseInt(c.value));
    });
    
    modal.show();
}

async function saveUser() {
    const id = document.getElementById('userId').value;
    const data = {
        id: id ? parseInt(id) : 0,
        fullName: document.getElementById('fullName').value,
        email: document.getElementById('email').value,
        password: document.getElementById('password').value,
        departmentId: document.getElementById('departmentId').value ? parseInt(document.getElementById('departmentId').value) : null,
        isActive: document.getElementById('isActive').checked,
        roleIds: Array.from(document.querySelectorAll('.role-checkbox:checked')).map(c => parseInt(c.value))
    };

    if (!data.fullName || !data.email) {
        showToast('Ad soyad ve email zorunludur', 'error');
        return;
    }

    try {
        showLoading();
        if (id) {
            await api.put(`/api/users/${id}`, data);
        } else {
            await api.post('/api/users', data);
        }
        modal.hide();
        await loadData();
        showToast('Kullanıcı kaydedildi');
    } catch (e) {
        showToast('Kayıt başarısız: ' + (e.message || ''), 'error');
    } finally {
        hideLoading();
    }
}

async function deleteUser(id) {
    if (!confirmDelete()) return;
    
    try {
        showLoading();
        await api.delete(`/api/users/${id}`);
        await loadData();
        showToast('Kullanıcı silindi');
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

// Make functions globally available
window.openModal = openModal;
window.editUser = editUser;
window.saveUser = saveUser;
window.deleteUser = deleteUser;