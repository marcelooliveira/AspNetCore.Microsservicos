var el = document.querySelector('.notification');

document.querySelector('.button-notification').addEventListener('click', function () {
    var count = Number(el.getAttribute('data-count')) || 0;
    el.setAttribute('data-count', count + 1);
    el.classList.remove('notify');
    el.offsetWidth = el.offsetWidth;
    el.classList.add('notify');
    if (count === 0) {
        el.classList.add('show-count');
    }
}, false);