window.select2Interop = {
    init: function (elementId, dotNetHelper, placeholderText) {
        var el = $('#' + elementId);

        // Evitar doble inicialización
        if (el.hasClass("select2-hidden-accessible")) {
            return;
        }

        // Inicializar Select2
        el.select2({
            placeholder: placeholderText,
            allowClear: true,

            minimumResultsForSearch: 0,

            // 2. Permite que el usuario escriba un valor nuevo
            tags: true,

            // 3. (Opcional) Formatea cómo se ve el nuevo tag
            createTag: function (params) {
                return {
                    id: params.term,
                    text: params.term,
                    newTag: true
                }
            }
            // --- FIN DE LA SOLUCIÓN ---
        });

        // Configurar un listener para cuando el usuario seleccione algo
        el.on('change.select2', function (e) {
            var selectedValue = $(this).val();
            dotNetHelper.invokeMethodAsync('OnSelect2Changed', selectedValue);
        });
    },
)
    destroy: function (elementId) {
        var el = $('#' + elementId);
        if (el.data('select2')) {
            el.off('change.select2');
            el.select2('destroy');
        }
    }
};

window.descargarArchivo = function (fileName, contentType, base64Content) {
    console.log(`[JS][descargarArchivo] Intentando descargar archivo: ${fileName}, Tipo: ${contentType}`);
    try {
        const byteCharacters = atob(base64Content);
        const byteNumbers = new Array(byteCharacters.length);
        for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        const blob = new Blob([byteArray], { type: contentType });

        const url = URL.createObjectURL(blob);

        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
        console.log(`[JS][descargarArchivo] Descarga de archivo '${fileName}' iniciada con éxito.`);
    } catch (e) {
        console.error(`[JS][descargarArchivo] ERROR al descargar el archivo '${fileName}':`, e);
        // CAMBIO: Usar SweetAlert en lugar de alert nativo
        Swal.fire({
            icon: 'error',
            title: 'Error al descargar',
            text: `Error al descargar el archivo: ${e.message}`,
            confirmButtonColor: '#d33'
        });
    }
};

// sweet alerts
window.sweetAlert = {
    success: function (title, message) {
        return Swal.fire({
            icon: 'success',
            title: title,
            text: message,
            timer: 3000,
            showConfirmButton: false
        });
    },

    error: function (title, message) {
        return Swal.fire({
            icon: 'error',
            title: title,
            text: message,
            confirmButtonColor: '#d33'
        });
    },

    warning: function (title, message) {
        return Swal.fire({
            icon: 'warning',
            title: title,
            text: message,
            confirmButtonColor: '#f39c12'
        });
    },

    info: function (title, message) {
        return Swal.fire({
            icon: 'info',
            title: title,
            text: message,
            confirmButtonColor: '#3498db'
        });
    },

    validationError: function (title, errors) {
        let htmlContent = '<div class="text-start"><ul class="mb-0">';
        errors.forEach(error => {
            htmlContent += `<li>${error}</li>`;
        });
        htmlContent += '</ul></div>';

        return Swal.fire({
            icon: 'error',
            title: title,
            html: htmlContent,
            confirmButtonColor: '#d33',
            width: '600px'
        });
    }
};

// Logout y Session Management
window.sessionManager = {
    dotnetRef: null,
    inactivityTimeout: null,
    activityListeners: [],

    registerDotNet: function (dotnetRef) {
        console.log('📡 [sessionManager] DotNet ref registrado');
        this.dotnetRef = dotnetRef;
    },

    startListening: function () {
        console.log('👁️ [sessionManager] Iniciando escucha de actividad del usuario');

        const events = ['mousedown', 'keydown', 'scroll', 'touchstart', 'click'];
        
        events.forEach(event => {
            const handler = () => {
                console.log(`🖱️ [sessionManager] Actividad detectada: ${event}`);
                this.resetInactivityTimer();
            };

            document.addEventListener(event, handler, true);
            this.activityListeners.push({ event, handler });
        });
    },

    resetInactivityTimer: function () {
        if (this.dotnetRef) {
            try {
                this.dotnetRef.invokeMethodAsync('ResetInactivity').catch(err => {
                    console.warn('⚠️ [sessionManager] Error al resetear inactividad:', err);
                });
            } catch (err) {
                console.warn('⚠️ [sessionManager] Error al invocar ResetInactivity:', err);
            }
        }
    },

    stopListening: function () {
        console.log('🛑 [sessionManager] Deteniendo escucha de actividad');
        
        this.activityListeners.forEach(({ event, handler }) => {
            document.removeEventListener(event, handler, true);
        });
        
        this.activityListeners = [];

        if (this.dotnetRef) {
            this.dotnetRef.dispose();
            this.dotnetRef = null;
        }
    },

    showSessionWarning: function () {
        console.log('⚠️ [sessionManager] Mostrando alerta de sesión');
        
        return Swal.fire({
            icon: 'warning',
            title: '⚠️ Sesión por expirar',
            html: '<p>Tu sesión se cerrará en <strong>1 minuto</strong> por inactividad.</p><p>¿Deseas continuar?</p>',
            confirmButtonText: 'Continuar activo',
            cancelButtonText: 'Cerrar sesión',
            confirmButtonColor: '#0d6efd',
            cancelButtonColor: '#d33',
            allowOutsideClick: false,
            allowEscapeKey: false,
            showCancelButton: true
        }).then((result) => {
            if (result.isConfirmed) {
                console.log('✅ [sessionManager] Usuario seleccionó continuar');
                // Resetear inactividad
                this.resetInactivityTimer();
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                console.log('🚪 [sessionManager] Usuario seleccionó cerrar sesión');
                if (this.dotnetRef) {
                    this.dotnetRef.invokeMethodAsync('LogoutAsync');
                }
            }
        });
    }
};