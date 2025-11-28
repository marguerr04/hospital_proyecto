window.sessionManager = (function () {
    let dotNetRef = null;
    let listening = false;

    function registerDotNet(ref) {
        dotNetRef = ref;
    }

    function startListening() {
        if (listening) return;
        listening = true;
        const notify = () => {
            if (dotNetRef) {
                try { dotNetRef.invokeMethodAsync('ResetInactivity'); } catch (e) { console.warn(e); }
            }
        };
        window.addEventListener('mousemove', notify);
        window.addEventListener('mousedown', notify);
        window.addEventListener('click', notify);
        window.addEventListener('keydown', notify);
        window.addEventListener('scroll', notify);
        window.addEventListener('focus', notify);
        // también enviar una vez al arrancar
        notify();
    }

    function stopListening() {
        if (!listening) return;
        listening = false;
        // No mantenemos referencias a handlers específicos para removerlos;
        // recargar la página o dejar listening=false es suficiente.
    }

    return {
        registerDotNet,
        startListening,
        stopListening
    };
})();

(function () {
    const INACTIVITY_MS = 5 * 60 * 1000;
    let timer = null;
    function reset() {
        if (timer) clearTimeout(timer);
        timer = setTimeout(() => {
            localStorage.removeItem('auth_token');
            localStorage.removeItem('auth_expires');
            window.location.href = '/login';
        }, INACTIVITY_MS);
    }
    ['mousemove','mousedown','click','keydown','scroll','touchstart'].forEach(ev => window.addEventListener(ev, reset));
    reset();
})();