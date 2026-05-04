document.addEventListener('DOMContentLoaded', () => {
    const sidebar = document.getElementById('adminSidebar');
    const toggle = document.getElementById('sidebarToggle');
    const backdrop = document.getElementById('sidebarBackdrop');

    const closeSidebar = () => {
        sidebar?.classList.remove('open');
        backdrop?.classList.remove('show');
    };

    toggle?.addEventListener('click', () => {
        sidebar?.classList.toggle('open');
        backdrop?.classList.toggle('show');
    });

    backdrop?.addEventListener('click', closeSidebar);
    document.querySelectorAll('.admin-sidebar a').forEach(link => {
        link.addEventListener('click', closeSidebar);
    });
});
