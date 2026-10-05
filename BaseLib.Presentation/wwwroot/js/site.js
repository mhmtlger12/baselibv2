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
