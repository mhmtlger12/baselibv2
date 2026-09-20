'use strict';

// Progressive enhancements; public navigation, search and scores work without JavaScript.
document.addEventListener('click', (event) => {
    const toggle = event.target.closest('[data-toggle]');
    if (!toggle) return;
    const target = document.getElementById(toggle.dataset.toggle);
    if (!target) return;
    target.hidden = !target.hidden;
    toggle.setAttribute('aria-expanded', String(!target.hidden));
});

for (const slider of document.querySelectorAll('[data-slider]')) {
    const slides = [...slider.querySelectorAll('[data-slide]')];
    const buttons = [...slider.querySelectorAll('[data-slide-index]')];
    const pause = slider.querySelector('[data-slider-pause]');
    const reducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)');
    let index = 0;
    let paused = reducedMotion.matches;
    function show(next) {
        index = next;
        slides.forEach((slide, i) => { slide.hidden = i !== index; });
        buttons.forEach((button, i) => {
            button.setAttribute('aria-current', String(i === index));
            button.style.opacity = i === index ? '1' : '.5';
        });
    }
    function updatePause() {
        pause.textContent = paused ? 'Oynat' : 'Durdur';
        pause.setAttribute('aria-pressed', String(paused));
    }
    buttons.forEach((button, i) => button.addEventListener('click', () => show(i)));
    pause.addEventListener('click', () => { paused = !paused; updatePause(); });
    setInterval(() => {
        if (!paused && !document.hidden && !slider.matches(':hover, :focus-within') && slides.length > 1) show((index + 1) % slides.length);
    }, 6000);
    updatePause();
    show(0);
}
function updateCountdowns() {
    for (const element of document.querySelectorAll('[data-countdown]')) {
        element.textContent = String(Math.max(0, Math.ceil((new Date(element.dataset.countdown) - Date.now()) / 86400000)));
    }
}
updateCountdowns();
setInterval(updateCountdowns, 60000);

// Mock comments remain in the current page only. Only a small formatting allowlist is copied.
function safeFormattedContent(source) {
    const fragment = document.createDocumentFragment();
    function append(node, target) {
        if (node.nodeType === Node.TEXT_NODE) { target.append(document.createTextNode(node.textContent)); return; }
        if (node.nodeType !== Node.ELEMENT_NODE || ['SCRIPT', 'STYLE'].includes(node.tagName)) return;
        const allowed = ['B', 'STRONG', 'I', 'EM', 'UL', 'OL', 'LI', 'BR', 'DIV', 'P'].includes(node.tagName);
        const container = allowed ? document.createElement(node.tagName.toLowerCase()) : document.createDocumentFragment();
        [...node.childNodes].forEach(child => append(child, container));
        target.append(container);
    }
    [...source.childNodes].forEach(node => append(node, fragment));
    return fragment;
}
for (const section of document.querySelectorAll('[data-comments]')) {
    const updateCount = () => { section.querySelector('[data-comment-count]').textContent = `(${section.querySelectorAll('[data-comment]').length})`; };
    section.addEventListener('click', event => {
        const like = event.target.closest('[data-like]');
        if (like) {
            const liked = like.getAttribute('aria-pressed') !== 'true';
            like.setAttribute('aria-pressed', String(liked));
            like.querySelector('span').textContent = String(Number(like.dataset.likes) + (liked ? 1 : 0));
        }
        const format = event.target.closest('[data-format]');
        if (format) {
            const editor = format.closest('form').querySelector('[data-editor]');
            editor.focus();
            document.execCommand(format.dataset.format, false);
        }
    });
    section.addEventListener('submit', event => {
        const form = event.target.closest('[data-comment-form]');
        if (!form) return;
        event.preventDefault();
        const editor = form.querySelector('[data-editor]');
        const body = editor.textContent.trim();
        if (!form.reportValidity() || !body || body.length > 4000) {
            section.querySelector('[data-comment-status]').textContent = 'Lütfen en fazla 4000 karakterlik bir yorum yazın.';
            editor.focus();
            return;
        }
        const values = new FormData(form);
        let name = values.get('name').trim();
        if (values.get('hideName')) name = name.split(/\s+/).map(part => part[0] + '*'.repeat(Math.max(2, part.length - 1))).join(' ');
        const node = document.getElementById('comment-template').content.cloneNode(true);
        node.querySelector('[data-author]').textContent = name;
        node.querySelector('[data-avatar]').textContent = name[0];
        node.querySelector('[data-body]').append(safeFormattedContent(editor));
        const parent = form.closest('[data-comment]');
        const list = parent ? parent.querySelector(':scope > [data-replies]') : section.querySelector('[data-comment-list]');
        list.prepend(node);
        form.reset();
        editor.replaceChildren();
        section.querySelector('[data-comment-status]').textContent = 'Yorumunuz bu örnek sayfaya eklendi.';
        updateCount();
    });
    updateCount();
}
