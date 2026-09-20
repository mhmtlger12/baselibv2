'use strict';

const sidebar = document.getElementById('admin-sidebar');
const sidebarButton = document.querySelector('[data-sidebar-toggle]');
const overlay = document.getElementById('sidebar-overlay');
function setSidebar(open) {
    sidebar?.classList.toggle('-translate-x-full', !open);
    sidebarButton?.setAttribute('aria-expanded', String(open));
    if (overlay) overlay.hidden = !open;
    if (!open) sidebarButton?.focus();
}
sidebarButton?.addEventListener('click', () => setSidebar(sidebarButton.getAttribute('aria-expanded') !== 'true'));
overlay?.addEventListener('click', () => setSidebar(false));
document.addEventListener('keydown', event => { if (event.key === 'Escape' && overlay && !overlay.hidden) setSidebar(false); });

document.addEventListener('click', event => {
    const button = event.target.closest('[data-toggle]');
    if (!button) return;
    const target = document.getElementById(button.dataset.toggle);
    if (!target) return;
    target.hidden = !target.hidden;
    button.setAttribute('aria-expanded', String(!target.hidden));
});

for (const panel of document.querySelectorAll('[data-filter-table]')) {
    const input = panel.querySelector('[data-table-search]');
    input?.addEventListener('input', () => {
        const query = input.value.trim().toLocaleLowerCase('tr-TR');
        let count = 0;
        for (const row of panel.querySelectorAll('[data-row]')) {
            row.hidden = !row.textContent.toLocaleLowerCase('tr-TR').includes(query);
            if (!row.hidden) count++;
        }
        const empty = panel.querySelector('[data-empty]');
        if (empty) empty.hidden = count > 0;
        panel.querySelector('[data-filter-status]').textContent = `${count} kayıt bulundu.`;
    });
}

for (const dialog of document.querySelectorAll('[data-edit-dialog]')) {
    const cancel = dialog.querySelector('[data-dialog-cancel]');
    dialog.removeAttribute('open');
    dialog.showModal();
    dialog.addEventListener('cancel', event => { event.preventDefault(); window.location.assign(cancel.href); });
    dialog.addEventListener('click', event => {
        const bounds = dialog.getBoundingClientRect();
        if (event.target === dialog && (event.clientX < bounds.left || event.clientX > bounds.right || event.clientY < bounds.top || event.clientY > bounds.bottom)) window.location.assign(cancel.href);
    });
    const invalid = dialog.querySelector('.input-validation-error');
    if (invalid) { invalid.setAttribute('aria-invalid', 'true'); invalid.focus(); }
}

for (const group of document.querySelectorAll('[data-permission-group]')) {
    const all = group.querySelector('[data-select-group]');
    const items = [...group.querySelectorAll('[name="PermissionIds"]')];
    const sync = () => { all.checked = items.every(item => item.checked); all.indeterminate = items.some(item => item.checked) && !all.checked; };
    all.addEventListener('change', () => { items.forEach(item => { item.checked = all.checked; }); sync(); });
    items.forEach(item => item.addEventListener('change', sync));
    sync();
}

document.addEventListener('submit', event => {
    const form = event.target;
    if (form.dataset.confirm && !window.confirm(form.dataset.confirm)) { event.preventDefault(); return; }
    if (!form.checkValidity()) return;
    const submit = event.submitter;
    if (submit) {
        submit.disabled = true;
        submit.setAttribute('aria-busy', 'true');
        if (submit.dataset.savingText) {
            submit.dataset.originalLabel = submit.textContent;
            submit.textContent = submit.dataset.savingText;
        }
    }
});
window.addEventListener('pageshow', () => {
    document.querySelectorAll('[aria-busy="true"]').forEach(button => {
        button.disabled = false;
        button.removeAttribute('aria-busy');
        if (button.dataset.originalLabel) button.textContent = button.dataset.originalLabel;
    });
});

for (const input of document.querySelectorAll('[data-image-input]')) {
    const preview = input.closest('form').querySelector('[data-image-preview]');
    if (!preview) continue;
    const image = preview.querySelector('img');
    input.addEventListener('input', () => {
        let url;
        try { url = new URL(input.value.trim(), window.location.origin); } catch { /* Incomplete address. */ }
        const valid = input.value.trim() && url && ['http:', 'https:'].includes(url.protocol);
        preview.hidden = !valid;
        if (valid) image.src = url.href;
        else image.removeAttribute('src');
    });
}

const chart = document.querySelector('[data-role-chart]');
if (chart) {
    const context = chart.getContext('2d');
    const entries = [...document.querySelectorAll('[data-role-value]')];
    const colors = ['#0f9f8f', '#0a192f', '#f59e0b', '#6366f1', '#10b981', '#ef4444'];
    const total = entries.reduce((sum, item) => sum + Number(item.dataset.roleValue), 0);
    let angle = -Math.PI / 2;
    context.lineWidth = 18;
    context.strokeStyle = '#eef3fb';
    context.beginPath(); context.arc(65, 65, 52, 0, 2 * Math.PI); context.stroke();
    entries.forEach((item, index) => {
        const next = angle + (total ? Number(item.dataset.roleValue) / total : 0) * 2 * Math.PI;
        context.strokeStyle = colors[index % colors.length];
        item.querySelector('span').style.backgroundColor = colors[index % colors.length];
        context.beginPath(); context.arc(65, 65, 52, angle, next); context.stroke();
        angle = next;
    });
    context.fillStyle = '#101d3a'; context.font = '700 22px sans-serif'; context.textAlign = 'center'; context.textBaseline = 'middle'; context.fillText(String(total), 65, 65);
}
