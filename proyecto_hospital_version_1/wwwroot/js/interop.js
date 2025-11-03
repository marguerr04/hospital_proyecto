window.select2Interop = {
    // Función para INICIALIZAR Select2
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

            // --- ¡AQUÍ ESTÁ LA SOLUCIÓN! ---

            // 1. Fuerza a que la barra de búsqueda SIEMPRE aparezca
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
            // Llamar al método de C# 'OnSelect2Changed'
            dotNetHelper.invokeMethodAsync('OnSelect2Changed', selectedValue);
        });
    },

    // Función para DESTRUIR Select2 (limpieza)
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
        // Tu código anterior usaba "data:..." URL directamente, lo cual tiene límites de tamaño.
        // La implementación con Blob y URL.createObjectURL es más robusta para PDFs más grandes.
        // Usaremos la versión con Blob, que es la que te di en el código anterior.

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
        document.body.appendChild(a); // Es bueno añadirlo al DOM para algunos navegadores
        a.click();
        document.body.removeChild(a); // Limpiar
        URL.revokeObjectURL(url); // Liberar recursos
        console.log(`[JS][descargarArchivo] Descarga de archivo '${fileName}' iniciada con éxito.`);
    } catch (e) {
        console.error(`[JS][descargarArchivo] ERROR al descargar el archivo '${fileName}':`, e);
        // Muestra una alerta con el error completo en el navegador
        alert(`Error al descargar el archivo: ${e.message}\nRevise la consola del navegador para más detalles.`);
    }
};