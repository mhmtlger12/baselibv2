// Common API functions for admin panel
const api = {
    async get(url) {
        const token = getCookie('AccessToken');
        const response = await fetch(url, {
            headers: {
                'Authorization': token ? `Bearer ${token}` : '',
                'Content-Type': 'application/json'
            }
        });
        const data = await response.json();
        return data.data;
    },

    async post(url, data) {
        const token = getCookie('AccessToken');
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Authorization': token ? `Bearer ${token}` : '',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });
        const result = await response.json();
        return result.data;
    },

    async put(url, data) {
        const token = getCookie('AccessToken');
        const response = await fetch(url, {
            method: 'PUT',
            headers: {
                'Authorization': token ? `Bearer ${token}` : '',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });
        const result = await response.json();
        return result.data;
    },

    async delete(url) {
        const token = getCookie('AccessToken');
        const response = await fetch(url, {
            method: 'DELETE',
            headers: {
                'Authorization': token ? `Bearer ${token}` : ''
            }
        });
        return response.ok;
    }
};

// Cookie helpers
function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
    return null;
}

// Toast notifications
function showToast(message, type = 'success') {
    const toast = document.createElement('div');
    toast.className = `toast-notification toast-${type}`;
    toast.innerHTML = `
        <i class="bi bi-${type === 'success' ? 'check-circle' : 'exclamation-circle'}"></i>
        <span>${message}</span>
    `;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 3000);
}

// Confirm dialog
function confirmDelete(message = 'Silmek istediğinize emin misiniz?') {
    return confirm(message);
}

// Loading spinner
function showLoading() {
    const loader = document.createElement('div');
    loader.id = 'global-loader';
    loader.className = 'global-loader';
    loader.innerHTML = '<div class="spinner"></div>';
    document.body.appendChild(loader);
}

function hideLoading() {
    const loader = document.getElementById('global-loader');
    if (loader) loader.remove();
}

// Initialize common functionality
document.addEventListener('DOMContentLoaded', function() {
    // Add loading styles if not present
    if (!document.getElementById('global-styles')) {
        const style = document.createElement('style');
        style.id = 'global-styles';
        style.textContent = `
            .global-loader { position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center; z-index: 9999; }
            .spinner { width: 40px; height: 40px; border: 4px solid #f3f3f3; border-top: 4px solid #667eea; border-radius: 50%; animation: spin 1s linear infinite; }
            @keyframes spin { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }
            .toast-notification { position: fixed; top: 20px; right: 20px; padding: 15px 20px; border-radius: 8px; color: white; display: flex; align-items: center; gap: 10px; z-index: 10000; animation: slideIn 0.3s ease; }
            .toast-success { background: #10b981; }
            .toast-error { background: #ef4444; }
            @keyframes slideIn { from { transform: translateX(100%); } to { transform: translateX(0); } }
        `;
        document.head.appendChild(style);
    }
});