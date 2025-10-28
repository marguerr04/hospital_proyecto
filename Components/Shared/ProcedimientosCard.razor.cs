using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace proyecto_hospital_version_1.Components.Shared
{
    // NOTA IMPORTANTE:
    // NO escribas ": ComponentBase" aquí. 
    // El .razor ya lo hace por nosotros. Este fue el error CS0263 anterior.
    public partial class ProcedimientosCard
    {
        // --- PARÁMETROS DE ENTRADA (Datos que recibe del padre) ---

        // 1. Recibe la lista de procedimientos para mostrarla
        [Parameter]
        public List<string> ListaProcedimientos { get; set; } = new List<string>();


        // --- PARÁMETROS DE SALIDA (Eventos que notifica al padre) ---

        // 2. Notifica al padre que el botón "Agregar" fue presionado
        [Parameter]
        public EventCallback OnAgregarPresionado { get; set; }


        // --- MÉTODOS INTERNOS ---

        // 3. Este método privado se activa con el clic del botón
        //    y su único trabajo es "disparar" el evento de salida.
        private async Task HandleAgregarClick()
        {
            await OnAgregarPresionado.InvokeAsync();
        }
    }
}