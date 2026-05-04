document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('[data-progress]').forEach(item => {
        const value = Number.parseInt(item.dataset.progress || '0', 10);
        item.style.width = `${Math.max(0, Math.min(100, value))}%`;
    });
});
