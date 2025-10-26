using Microsoft.AspNetCore.Components;
using proyecto_hospital_version_1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace proyecto_hospital_version_1.Components.Shared
{
    public partial class Paso3GenerarSolicitudCard : ComponentBase
    {
        [Parameter]
        public PacienteHospital? Paciente { get; set; }

        [Parameter]
        public string? ProcedimientoPrincipal { get; set; }

        [Parameter]
        public string? DiagnosticoPrincipal { get; set; }

        [Parameter]
        public string? ProcedenciaPrincipal { get; set; }

        [Parameter]
        public bool EsGes { get; set; }

        // Para comunicar el estado de completitud al padre
        [Parameter]
        public bool IsReadyToProceed { get; set; }
        [Parameter]
        public EventCallback<bool> IsReadyToProceedChanged { get; set; }

        // Variables internas del componente
        public List<string> EquiposSeleccionados { get; set; } = new List<string>();
        public string TipoMesaSeleccionado { get; set; } = string.Empty;
        public string SalaOperaciones { get; set; } = string.Empty;
        public int TiempoEstimado { get; set; } = 0;
        public bool EvaluacionAnestesica { get; set; } = false;
        public bool Transfusiones { get; set; } = false;
        public List<string> ComorbilidadesSeleccionadas { get; set; } = new List<string>();
        public string ComentariosAdicionales { get; set; } = string.Empty;

        // Variables para controlar los modales
        public bool MostrarModalEquipos { get; set; } = false;
        public bool MostrarModalComorbilidades { get; set; } = false;

        // Lista de opciones disponibles para equipos
        public List<string> OpcionesEquipos { get; set; } = new List<string>
        {
            "Arco C", "Torre Lap", "Sutura mecánica", "Microscopio", "Láser", "Bisturí armónico",
            "Bombas de infusión", "Monitor de signos vitales" // Añadir más para tener opciones en el modal
        };

        // Lista de opciones disponibles para comorbilidades
        public List<string> OpcionesComorbilidades { get; set; } = new List<string>
        {
            "Diabetes Mellitus", "Hipertensión Arterial", "Asma", "Insuficiencia Renal Crónica",
            "Cardiopatía Isquémica", "EPOC", "Obesidad Mórbida", "Dislipidemia", "Hipotiroidismo" // Añadir más
        };


        protected override void OnParametersSet()
        {
            // Solo llamar a CheckCompletion si Paciente no es nulo, para evitar errores iniciales
            if (Paciente != null)
            {
                CheckCompletion();
            }
        }

        // Lógica de validación interna
        public async Task CheckCompletion()
        {
            // La validación ahora debe considerar los equipos principales como seleccionados si están en la lista
            // Y que al menos 1 equipo (ya sea de los principales o del modal) esté seleccionado.
            bool ready = Paciente != null &&
                         EquiposSeleccionados.Any() &&
                         !string.IsNullOrWhiteSpace(TipoMesaSeleccionado) &&
                         !string.IsNullOrWhiteSpace(SalaOperaciones) &&
                         TiempoEstimado > 0;

            if (IsReadyToProceed != ready)
            {
                IsReadyToProceed = ready;
                await IsReadyToProceedChanged.InvokeAsync(IsReadyToProceed);
            }
            StateHasChanged(); // Asegurar que el UI se actualice con los cambios de estado.
        }

        // --- Métodos para Modales de Equipos ---
        public void AbrirModalEquipos()
        {
            MostrarModalEquipos = true;
            StateHasChanged();
        }

        public void CerrarModalEquipos()
        {
            MostrarModalEquipos = false;
            CheckCompletion();
            StateHasChanged();
        }

        public void ToggleEquipoDesdeModal(string equipo, bool isChecked)
        {
            if (isChecked && !EquiposSeleccionados.Contains(equipo))
            {
                EquiposSeleccionados.Add(equipo);
            }
            else if (!isChecked && EquiposSeleccionados.Contains(equipo))
            {
                EquiposSeleccionados.Remove(equipo);
            }
            StateHasChanged(); // Forzar la actualización del UI en el modal
        }

        // Método para los botones (Arco C, Torre Lap, Sutura mecánica)
        public void ToggleEquipoDirecto(string equipo)
        {
            if (EquiposSeleccionados.Contains(equipo))
            {
                EquiposSeleccionados.Remove(equipo);
            }
            else
            {
                EquiposSeleccionados.Add(equipo);
            }
            CheckCompletion(); // Validar después de cambiar
            StateHasChanged();
        }

        public void RemoverEquipo(string equipo)
        {
            EquiposSeleccionados.Remove(equipo);
            CheckCompletion();
            StateHasChanged();
        }

        // --- Métodos para Modales de Comorbilidades ---
        public void AbrirModalComorbilidades()
        {
            MostrarModalComorbilidades = true;
            StateHasChanged();
        }

        public void CerrarModalComorbilidades()
        {
            MostrarModalComorbilidades = false;
            CheckCompletion();
            StateHasChanged();
        }

        public void ToggleComorbilidadDesdeModal(string comorbilidad, bool isChecked)
        {
            if (isChecked && !ComorbilidadesSeleccionadas.Contains(comorbilidad))
            {
                ComorbilidadesSeleccionadas.Add(comorbilidad);
            }
            else if (!isChecked && ComorbilidadesSeleccionadas.Contains(comorbilidad))
            {
                ComorbilidadesSeleccionadas.Remove(comorbilidad);
            }
            StateHasChanged(); // Forzar la actualización del UI en el modal
        }

        public void RemoverComorbilidad(string comorbilidad)
        {
            ComorbilidadesSeleccionadas.Remove(comorbilidad);
            CheckCompletion();
            StateHasChanged();
        }

        // Métodos para los Toggle Buttons directos del UI
        public void SeleccionarTipoMesa(string tipo)
        {
            TipoMesaSeleccionado = tipo;
            CheckCompletion();
            StateHasChanged();
        }

        // Estos métodos no son estrictamente necesarios si usas @bind en los switches,
        // pero son útiles para llamar a CheckCompletion explícitamente.
        public void OnEvaluacionAnestesicaChanged(ChangeEventArgs e)
        {
            EvaluacionAnestesica = (bool)e.Value;
            CheckCompletion();
        }

        public void OnTransfusionesChanged(ChangeEventArgs e)
        {
            Transfusiones = (bool)e.Value;
            CheckCompletion();
        }
    }
}