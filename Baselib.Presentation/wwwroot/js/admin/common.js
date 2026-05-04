const api = {
    async get(url) {
        return request(url);
    },

    async post(url, data) {
        return request(url, {
            method: 'POST',
            body: JSON.stringify(data ?? {})
        });
    },

    async put(url, data) {
        return request(url, {
            method: 'PUT',
            body: JSON.stringify(data ?? {})
        });
    },

    async delete(url) {
        await request(url, { method: 'DELETE' });
        return true;
    }
};

async function request(url, options = {}) {
    const response = await fetch(url, {
        ...options,
        headers: {
            'Content-Type': 'application/json',
            ...(options.headers || {})
        }
    });

    const text = await response.text();
    const payload = text ? JSON.parse(text) : null;

    if (!response.ok || payload?.success === false) {
        throw new Error(payload?.message || 'İşlem tamamlanamadı');
    }

    return payload?.data ?? payload;
}

function showToast(message, type = 'success') {
    const toast = document.createElement('div');
    toast.className = `toast-notification toast-${type}`;
    toast.innerHTML = `
        <i class="bi bi-${type === 'success' ? 'check-circle' : 'exclamation-circle'}"></i>
        <span>${escapeHtml(message)}</span>
    `;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 3200);
}

function confirmDelete(message = 'Silmek istediğinize emin misiniz?') {
    return confirm(message);
}

function showLoading() {
    if (document.getElementById('global-loader')) return;

    const loader = document.createElement('div');
    loader.id = 'global-loader';
    loader.className = 'global-loader';
    loader.innerHTML = '<div class="spinner" role="status" aria-label="Yükleniyor"></div>';
    document.body.appendChild(loader);
}

function hideLoading() {
    document.getElementById('global-loader')?.remove();
}

function escapeHtml(text) {
    if (text === null || text === undefined) return '';

    const div = document.createElement('div');
    div.textContent = String(text);
    return div.innerHTML;
}

window.api = api;
window.showToast = showToast;
window.confirmDelete = confirmDelete;
window.showLoading = showLoading;
window.hideLoading = hideLoading;
window.escapeHtml = escapeHtml;
