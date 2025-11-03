using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace proyecto_hospital_version_1.Shared
{
    // LA CLAVE ESTÁ AQUÍ: debe ser 'public partial class'
    // y el nombre 'CriteriosPriorizacion' debe coincidir
    // exactamente con el nombre del archivo .razor
    public partial class CriteriosPriorizacion
    {
        [Parameter]
        public string CriterioSeleccionado { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<string> CriterioSeleccionadoChanged { get; set; }

        private HashSet<int> expandedGroups = new HashSet<int>() { 1, 2, 3, 4 }; // Todos expandidos por defecto

        private void ToggleGroup(int groupNumber)
        {
            if (expandedGroups.Contains(groupNumber))
                expandedGroups.Remove(groupNumber);
            else
                expandedGroups.Add(groupNumber);
        }

        private async Task OnCriterioSeleccionado(string? criterio)
        {
            if (!string.IsNullOrEmpty(criterio))
            {
                // Notifica al componente padre (HospitalProbe) sobre el cambio
                await CriterioSeleccionadoChanged.InvokeAsync(criterio);
            }
        }
    }
}

