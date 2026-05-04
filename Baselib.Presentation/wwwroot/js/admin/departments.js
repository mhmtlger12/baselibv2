let modal;
let departments = [];

document.addEventListener('DOMContentLoaded', async () => {
    modal = new bootstrap.Modal(document.getElementById('deptModal'));
    document.getElementById('openDeptModal')?.addEventListener('click', openModal);
    document.getElementById('saveDeptButton')?.addEventListener('click', saveDept);
    await loadData();
});

async function loadData() {
    try {
        showLoading();
        departments = await api.get('/api/departments') || [];
        renderTree();
        renderParentSelect();
    } catch (error) {
        showToast(error.message || 'Departmanlar yüklenemedi', 'error');
    } finally {
        hideLoading();
    }
}

function renderTree() {
    const container = document.getElementById('departmentTree');
    const roots = departments.filter(dept => !dept.parentDepartmentId);
    container.innerHTML = roots.length ? renderNodes(roots) : '<p class="text-muted mb-0">Departman bulunamadı.</p>';
}

function renderNodes(nodes) {
    return '<ul>' + nodes.map(dept => {
        const children = departments.filter(item => item.parentDepartmentId === dept.id);
        const hasChildren = children.length > 0;

        return `
            <li>
                <div class="tree-content">
                    <button type="button" class="tree-toggle ${hasChildren ? '' : 'empty'}" onclick="toggleNode(this)">
                        <i class="bi bi-chevron-right"></i>
                    </button>
                    <span class="tree-title">${escapeHtml(dept.name)}</span>
                    <span class="tree-meta">${escapeHtml(dept.code)}</span>
                    <span class="badge bg-${dept.isActive ? 'success' : 'secondary'}">${dept.isActive ? 'Aktif' : 'Pasif'}</span>
                    <div class="tree-actions">
                        <button class="btn btn-sm btn-warning" onclick="editDept(${dept.id})" title="Düzenle"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-sm btn-danger" onclick="deleteDept(${dept.id})" title="Sil"><i class="bi bi-trash"></i></button>
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
    const options = departments
        .filter(dept => dept.id !== excludeId)
        .map(dept => `<option value="${dept.id}">${escapeHtml(dept.name)}</option>`)
        .join('');

    select.innerHTML = '<option value="">Üst departman yok</option>' + options;
}

function openModal() {
    document.getElementById('modalTitle').textContent = 'Yeni Departman';
    document.getElementById('deptId').value = '';
    document.getElementById('name').value = '';
    document.getElementById('code').value = '';
    document.getElementById('parentId').value = '';
    document.getElementById('isActive').checked = true;
    renderParentSelect();
    modal.show();
}

function editDept(id) {
    const dept = departments.find(item => item.id === id);
    if (!dept) return;

    document.getElementById('modalTitle').textContent = 'Departman Düzenle';
    document.getElementById('deptId').value = dept.id;
    document.getElementById('name').value = dept.name || '';
    document.getElementById('code').value = dept.code || '';
    document.getElementById('isActive').checked = dept.isActive ?? true;
    renderParentSelect(dept.id);
    document.getElementById('parentId').value = dept.parentDepartmentId || '';
    modal.show();
}

async function saveDept() {
    const id = document.getElementById('deptId').value;
    const data = {
        name: document.getElementById('name').value.trim(),
        code: document.getElementById('code').value.trim(),
        parentDepartmentId: getNullableInt('parentId'),
        isActive: document.getElementById('isActive').checked
    };

    if (!data.name || !data.code) {
        showToast('Departman adı ve kodu zorunludur', 'error');
        return;
    }

    try {
        showLoading();
        if (id) {
            await api.put(`/api/departments/${id}`, data);
        } else {
            await api.post('/api/departments', data);
        }

        modal.hide();
        await loadData();
        showToast('Departman kaydedildi');
    } catch (error) {
        showToast(error.message || 'Kayıt başarısız', 'error');
    } finally {
        hideLoading();
    }
}

async function deleteDept(id) {
    if (!confirmDelete()) return;

    try {
        showLoading();
        await api.delete(`/api/departments/${id}`);
        await loadData();
        showToast('Departman silindi');
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
window.editDept = editDept;
window.saveDept = saveDept;
window.deleteDept = deleteDept;
window.toggleNode = toggleNode;
