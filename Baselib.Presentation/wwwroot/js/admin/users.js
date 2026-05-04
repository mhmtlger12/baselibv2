let users = [];
let roles = [];
let departments = [];
let modal;

document.addEventListener('DOMContentLoaded', async () => {
    modal = new bootstrap.Modal(document.getElementById('userModal'));
    document.getElementById('openUserModal')?.addEventListener('click', openModal);
    document.getElementById('saveUserButton')?.addEventListener('click', saveUser);
    await loadData();
});

async function loadData() {
    try {
        showLoading();
        const results = await Promise.allSettled([
            api.get('/api/users'),
            api.get('/api/roles'),
            api.get('/api/departments')
        ]);

        if (results[0].status === 'rejected') {
            throw new Error(results[0].reason.message || 'Kullanıcılar yüklenemedi');
        }

        users = results[0].value || [];
        roles = results[1].status === 'fulfilled' ? (results[1].value || []) : [];
        departments = results[2].status === 'fulfilled' ? (results[2].value || []) : [];

        renderTable();
        renderDepartments();
        renderRoles();
    } catch (error) {
        showToast(error.message || 'Veriler yüklenirken hata oluştu', 'error');
    } finally {
        hideLoading();
    }
}

function renderTable() {
    const tbody = document.querySelector('#userTable tbody');
    if (!tbody) return;

    tbody.innerHTML = users.map(user => `
        <tr>
            <td>${escapeHtml(user.fullName || `${user.firstName || ''} ${user.lastName || ''}`.trim())}</td>
            <td>${escapeHtml(user.username)}</td>
            <td>${escapeHtml(user.email)}</td>
            <td>${escapeHtml(user.departmentName || '-')}</td>
            <td>${renderRoleBadges(user.roles)}</td>
            <td>
                <span class="badge ${user.isActive ? 'bg-success' : 'bg-secondary'}">
                    ${user.isActive ? 'Aktif' : 'Pasif'}
                </span>
            </td>
            <td>
                <button class="btn btn-sm btn-warning" onclick="editUser(${user.id})" title="Düzenle">
                    <i class="bi bi-pencil"></i>
                </button>
                <button class="btn btn-sm btn-danger" onclick="deleteUser(${user.id})" title="Sil">
                    <i class="bi bi-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

function renderRoleBadges(roleNames = []) {
    if (!roleNames.length) return '<span class="text-muted">Rol yok</span>';
    return roleNames.map(role => `<span class="badge bg-primary me-1">${escapeHtml(role)}</span>`).join('');
}

function renderDepartments() {
    const select = document.getElementById('departmentId');
    if (!select) return;

    select.innerHTML = '<option value="">Seçiniz</option>' +
        departments.map(dept => `<option value="${dept.id}">${escapeHtml(dept.name)}</option>`).join('');
}

function renderRoles() {
    const container = document.getElementById('rolesList');
    if (!container) return;

    container.innerHTML = roles.map(role => `
        <div class="col-md-6">
            <div class="form-check">
                <input class="form-check-input role-checkbox" type="checkbox" value="${role.id}" id="role_${role.id}">
                <label class="form-check-label" for="role_${role.id}">${escapeHtml(role.name)}</label>
            </div>
        </div>
    `).join('');
}

function openModal() {
    document.getElementById('modalTitle').textContent = 'Yeni Kullanıcı';
    document.getElementById('userId').value = '';
    document.getElementById('username').value = '';
    document.getElementById('email').value = '';
    document.getElementById('firstName').value = '';
    document.getElementById('lastName').value = '';
    document.getElementById('phone').value = '';
    document.getElementById('password').value = '';
    document.getElementById('departmentId').value = '';
    document.getElementById('isActive').checked = true;
    document.querySelectorAll('.role-checkbox').forEach(item => item.checked = false);
    modal.show();
}

function editUser(id) {
    const user = users.find(item => item.id === id);
    if (!user) return;

    document.getElementById('modalTitle').textContent = 'Kullanıcı Düzenle';
    document.getElementById('userId').value = user.id;
    document.getElementById('username').value = user.username || '';
    document.getElementById('email').value = user.email || '';
    document.getElementById('firstName').value = user.firstName || '';
    document.getElementById('lastName').value = user.lastName || '';
    document.getElementById('phone').value = user.phone || '';
    document.getElementById('password').value = '';
    document.getElementById('departmentId').value = user.departmentId || '';
    document.getElementById('isActive').checked = user.isActive ?? true;

    document.querySelectorAll('.role-checkbox').forEach(item => {
        item.checked = (user.roleIds || []).includes(Number.parseInt(item.value, 10));
    });

    modal.show();
}

async function saveUser() {
    const id = document.getElementById('userId').value;
    const data = {
        username: document.getElementById('username').value.trim(),
        email: document.getElementById('email').value.trim(),
        password: document.getElementById('password').value,
        firstName: document.getElementById('firstName').value.trim(),
        lastName: document.getElementById('lastName').value.trim(),
        phone: document.getElementById('phone').value.trim(),
        departmentId: getNullableInt('departmentId'),
        isActive: document.getElementById('isActive').checked,
        roleIds: Array.from(document.querySelectorAll('.role-checkbox:checked')).map(item => Number.parseInt(item.value, 10))
    };

    if (!data.username || !data.email || !data.firstName || !data.lastName) {
        showToast('Kullanıcı adı, email, ad ve soyad zorunludur', 'error');
        return;
    }

    if (!id && !data.password) {
        showToast('Yeni kullanıcı için şifre zorunludur', 'error');
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
    } catch (error) {
        showToast(error.message || 'Kayıt başarısız', 'error');
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
    } catch (error) {
        showToast(error.message || 'Silme başarısız', 'error');
    } finally {
        hideLoading();
    }
}

function getNullableInt(elementId) {
    const value = document.getElementById(elementId).value;
    return value ? Number.parseInt(value, 10) : null;
}

window.openModal = openModal;
window.editUser = editUser;
window.saveUser = saveUser;
window.deleteUser = deleteUser;
