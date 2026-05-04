let modal;
let menus = [];
let permissions = [];

document.addEventListener('DOMContentLoaded', async () => {
    modal = new bootstrap.Modal(document.getElementById('menuModal'));
    document.getElementById('openMenuModal')?.addEventListener('click', openModal);
    document.getElementById('saveMenuButton')?.addEventListener('click', saveMenu);
    await loadData();
});

async function loadData() {
    try {
        showLoading();
        const results = await Promise.allSettled([
            api.get('/api/menus'),
            api.get('/api/permissions')
        ]);

        if (results[0].status === 'rejected') {
            throw new Error(results[0].reason.message || 'Menüler yüklenemedi');
        }

        menus = results[0].value || [];
        permissions = results[1].status === 'fulfilled' ? (results[1].value || []) : [];
        renderTree();
        renderParentSelect();
        renderPermissionSelect();
    } catch (error) {
        showToast(error.message || 'Menüler yüklenemedi', 'error');
    } finally {
        hideLoading();
    }
}

function renderTree() {
    const container = document.getElementById('menuTree');
    const roots = menus.filter(menu => !menu.parentId);
    container.innerHTML = roots.length ? renderNodes(roots) : '<p class="text-muted mb-0">Menü bulunamadı.</p>';
}

function renderNodes(nodes) {
    return '<ul>' + nodes.map(menu => {
        const children = menus.filter(item => item.parentId === menu.id);
        const hasChildren = children.length > 0;

        return `
            <li>
                <div class="tree-content">
                    <button type="button" class="tree-toggle ${hasChildren ? '' : 'empty'}" onclick="toggleNode(this)">
                        <i class="bi bi-chevron-right"></i>
                    </button>
                    <i class="bi ${escapeHtml(menu.icon || 'bi-circle')}"></i>
                    <span class="tree-title">${escapeHtml(menu.name)}</span>
                    ${menu.url ? `<span class="tree-meta">${escapeHtml(menu.url)}</span>` : ''}
                    <span class="badge bg-info">Sıra: ${menu.order || 0}</span>
                    <span class="badge bg-${menu.isActive ? 'success' : 'secondary'}">${menu.isActive ? 'Aktif' : 'Pasif'}</span>
                    <div class="tree-actions">
                        <button class="btn btn-sm btn-warning" onclick="editMenu(${menu.id})" title="Düzenle"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-sm btn-danger" onclick="deleteMenu(${menu.id})" title="Sil"><i class="bi bi-trash"></i></button>
                    </div>
                </div>
                ${hasChildren ? `<div class="tree-children">${renderNodes(children)}</div>` : ''}
            </li>
        `;
    }).join('') + '</ul>';
}

function toggleNode(button) {
    const children = button.closest('.tree-content')?.nextElementSibling;
    if (!children?.classList.contains('tree-children')) return;

    children.classList.toggle('expanded');
    button.querySelector('i')?.classList.toggle('bi-chevron-down', children.classList.contains('expanded'));
    button.querySelector('i')?.classList.toggle('bi-chevron-right', !children.classList.contains('expanded'));
}

function renderParentSelect(excludeId = null) {
    const select = document.getElementById('parentId');
    const options = menus
        .filter(menu => menu.id !== excludeId)
        .map(menu => `<option value="${menu.id}">${escapeHtml(menu.name)}</option>`)
        .join('');

    select.innerHTML = '<option value="">Üst menü yok</option>' + options;
}

function renderPermissionSelect() {
    const select = document.getElementById('permissionId');
    const options = permissions
        .map(permission => `<option value="${permission.id}">${escapeHtml(permission.name)} (${escapeHtml(permission.code)})</option>`)
        .join('');

    select.innerHTML = '<option value="">İzin gerektirmez</option>' + options;
}

function openModal() {
    document.getElementById('modalTitle').textContent = 'Yeni Menü';
    document.getElementById('menuId').value = '';
    document.getElementById('name').value = '';
    document.getElementById('url').value = '';
    document.getElementById('icon').value = '';
    document.getElementById('order').value = '0';
    document.getElementById('parentId').value = '';
    document.getElementById('permissionId').value = '';
    document.getElementById('isActive').checked = true;
    renderParentSelect();
    modal.show();
}

function editMenu(id) {
    const menu = menus.find(item => item.id === id);
    if (!menu) return;

    document.getElementById('modalTitle').textContent = 'Menü Düzenle';
    document.getElementById('menuId').value = menu.id;
    document.getElementById('name').value = menu.name || '';
    document.getElementById('url').value = menu.url || '';
    document.getElementById('icon').value = menu.icon || '';
    document.getElementById('order').value = menu.order || 0;
    document.getElementById('isActive').checked = menu.isActive ?? true;
    renderParentSelect(menu.id);
    document.getElementById('parentId').value = menu.parentId || '';
    document.getElementById('permissionId').value = menu.permissionId || '';
    modal.show();
}

async function saveMenu() {
    const id = document.getElementById('menuId').value;
    const data = {
        name: document.getElementById('name').value.trim(),
        url: document.getElementById('url').value.trim(),
        icon: document.getElementById('icon').value.trim(),
        order: Number.parseInt(document.getElementById('order').value, 10) || 0,
        parentId: getNullableInt('parentId'),
        permissionId: getNullableInt('permissionId'),
        isActive: document.getElementById('isActive').checked
    };

    if (!data.name) {
        showToast('Menü adı zorunludur', 'error');
        return;
    }

    try {
        showLoading();
        if (id) {
            await api.put(`/api/menus/${id}`, data);
        } else {
            await api.post('/api/menus', data);
        }

        modal.hide();
        await loadData();
        showToast('Menü kaydedildi');
    } catch (error) {
        showToast(error.message || 'Kayıt başarısız', 'error');
    } finally {
        hideLoading();
    }
}

async function deleteMenu(id) {
    if (!confirmDelete()) return;

    try {
        showLoading();
        await api.delete(`/api/menus/${id}`);
        await loadData();
        showToast('Menü silindi');
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
window.editMenu = editMenu;
window.saveMenu = saveMenu;
window.deleteMenu = deleteMenu;
window.toggleNode = toggleNode;
