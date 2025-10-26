using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace proyecto_hospital_version_1.Components.Shared
{
    public partial class SelectorEquipos : ComponentBase
    {
        // --- PARÁMETROS PARA COMUNICACIÓN ---

        // 1. Recibe la lista de opciones (desde el padre)
        [Parameter]
        public List<string> OpcionesEquipos { get; set; } = new List<string>();

        // 2. Recibe y notifica cambios en la lista de seleccionados (Binding)
        [Parameter]
        public List<string> EquiposSeleccionados { get; set; } = new List<string>();

        [Parameter]
        public EventCallback<List<string>> EquiposSeleccionadosChanged { get; set; }

        // --- LÓGICA INTERNA (Movida desde el padre) ---
        public bool MostrarModalEquipos { get; set; } = false;

        public void AbrirModalEquipos()
        {
            MostrarModalEquipos = true;
            StateHasChanged();
        }

        // ¡MODIFICADO!
        public async Task CerrarModalEquipos()
        {
            MostrarModalEquipos = false;
            // Notificamos al padre que la lista pudo cambiar
            await EquiposSeleccionadosChanged.InvokeAsync(EquiposSeleccionados);
        }

        // ¡MODIFICADO!
        public async Task ToggleEquipoDesdeModal(string equipo, bool isChecked)
        {
            if (isChecked && !EquiposSeleccionados.Contains(equipo))
            {
                EquiposSeleccionados.Add(equipo);
            }
            else if (!isChecked && EquiposSeleccionados.Contains(equipo))
            {
                EquiposSeleccionados.Remove(equipo);
            }
            // No notificamos al padre aquí, solo al cerrar el modal
            StateHasChanged();
        }

        // ¡MODIFICADO!
        public async Task ToggleEquipoDirecto(string equipo)
        {
            if (EquiposSeleccionados.Contains(equipo))
            {
                EquiposSeleccionados.Remove(equipo);
            }
            else
            {
                EquiposSeleccionados.Add(equipo);
            }
            // Notificamos al padre inmediatamente
            await EquiposSeleccionadosChanged.InvokeAsync(EquiposSeleccionados);
        }

        // ¡MODIFICADO!
        public async Task RemoverEquipo(string equipo)
        {
            EquiposSeleccionados.Remove(equipo);
            // Notificamos al padre inmediatamente
            await EquiposSeleccionadosChanged.InvokeAsync(EquiposSeleccionados);
        }
    }
}