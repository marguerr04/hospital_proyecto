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