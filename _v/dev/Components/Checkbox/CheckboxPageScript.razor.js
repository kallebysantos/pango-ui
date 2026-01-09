export function onLoad() {
    document.querySelectorAll('input[type="checkbox"][indeterminate]').forEach(el => {
        el.dataset.indeterminate = el.getAttribute('indeterminate') === 'true';
    });
}

export function onUpdate() {
    onLoad();
}
