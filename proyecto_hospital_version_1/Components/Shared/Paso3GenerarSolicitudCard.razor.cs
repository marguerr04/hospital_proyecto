using Hospital.Api.DTOs;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace proyecto_hospital_version_1.Components.Shared
{
    public partial class Paso3GenerarSolicitudCard : ComponentBase
    {
        [Parameter]
        public PacienteDto? Paciente { get; set; }

        [Parameter]
        public string? ProcedimientoPrincipal { get; set; }

        [Parameter]
        public string? DiagnosticoPrincipal { get; set; }

        [Parameter]
        public string? ProcedenciaPrincipal { get; set; }

        [Parameter]
        public bool EsGes { get; set; }

        // --- PARÁMETROS CON EVENT CALLBACKS ---
        [Parameter]
        public List<string> EquiposSeleccionados { get; set; } = new List<string>();
        [Parameter]
        public EventCallback<List<string>> EquiposSeleccionadosChanged { get; set; }

        [Parameter]
        public string TipoMesaSeleccionado { get; set; } = "Estándar General";
        [Parameter]
        public EventCallback<string> TipoMesaSeleccionadoChanged { get; set; }

        [Parameter]
        public bool EvaluacionAnestesica { get; set; }
        [Parameter]
        public EventCallback<bool> EvaluacionAnestesicaChanged { get; set; }

        [Parameter]
        public bool Transfusiones { get; set; }
        [Parameter]
        public EventCallback<bool> TransfusionesChanged { get; set; }

        [Parameter]
        public string SalaOperaciones { get; set; } = "";
        [Parameter]
        public EventCallback<string> SalaOperacionesChanged { get; set; }

        [Parameter]
        public int? TiempoEstimado { get; set; }
        [Parameter]
        public EventCallback<int?> TiempoEstimadoChanged { get; set; }

        [Parameter]
        public List<string> ComorbilidadesSeleccionadas { get; set; } = new List<string>();
        [Parameter]
        public EventCallback<List<string>> ComorbilidadesSeleccionadasChanged { get; set; }

        [Parameter]
        public string ComentariosAdicionales { get; set; } = "";
        [Parameter]
        public EventCallback<string> ComentariosAdicionalesChanged { get; set; }

        // Variables para controlar modales
        public bool MostrarModalComorbilidades { get; set; } = false;

        // Lista de opciones disponibles para equipos
        public List<string> OpcionesEquipos { get; set; } = new List<string>
        {
            "Arco C", "Torre Lap", "Sutura mecánica", "Microscopio", "Láser", "Bisturí armónico"
        };

        // Lista de opciones disponibles para comorbilidades
        public List<string> OpcionesComorbilidades { get; set; } = new List<string>
        {
            "Diabetes Mellitus", "Hipertensión Arterial", "Asma", "Insuficiencia Renal Crónica",
            "Cardiopatía Isquémica", "EPOC", "Obesidad Mórbida", "Dislipidemia", "Hipotiroidismo"
        };

        // --- MÉTODOS PARA MANEJAR CAMBIOS ---
        private async Task OnEquiposSeleccionadosChanged(List<string> nuevosEquipos)
        {
            await EquiposSeleccionadosChanged.InvokeAsync(nuevosEquipos);
        }

        private async Task OnTipoMesaSeleccionadoChanged(string nuevoTipo)
        {
            await TipoMesaSeleccionadoChanged.InvokeAsync(nuevoTipo);
        }

        private async Task OnEvaluacionAnestesicaChanged(ChangeEventArgs e)
        {
            if (e.Value is bool valor)
            {
                await EvaluacionAnestesicaChanged.InvokeAsync(valor);
            }
        }

        private async Task OnTransfusionesChanged(ChangeEventArgs e)
        {
            if (e.Value is bool valor)
            {
                await TransfusionesChanged.InvokeAsync(valor);
            }
        }

        private async Task OnSalaOperacionesChanged(ChangeEventArgs e)
        {
            await SalaOperacionesChanged.InvokeAsync(e.Value?.ToString() ?? "");
        }

        private async Task OnTiempoEstimadoChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int tiempo) && tiempo > 0)
            {
                await TiempoEstimadoChanged.InvokeAsync(tiempo);
            }
            else
            {
                await TiempoEstimadoChanged.InvokeAsync(null);
            }
        }

        private async Task OnComentariosAdicionalesChanged(ChangeEventArgs e)
        {
            await ComentariosAdicionalesChanged.InvokeAsync(e.Value?.ToString() ?? "");
        }

        // --- MÉTODOS PARA MODALES ---
        public void AbrirModalComorbilidades()
        {
            MostrarModalComorbilidades = true;
            StateHasChanged();
        }

        public async Task CerrarModalComorbilidades()
        {
            MostrarModalComorbilidades = false;
            await ComorbilidadesSeleccionadasChanged.InvokeAsync(ComorbilidadesSeleccionadas);
            StateHasChanged();
        }

        public async Task ToggleComorbilidadDesdeModal(string comorbilidad, bool isChecked)
        {
            if (isChecked && !ComorbilidadesSeleccionadas.Contains(comorbilidad))
            {
                ComorbilidadesSeleccionadas.Add(comorbilidad);
            }
            else if (!isChecked && ComorbilidadesSeleccionadas.Contains(comorbilidad))
            {
                ComorbilidadesSeleccionadas.Remove(comorbilidad);
            }

            await ComorbilidadesSeleccionadasChanged.InvokeAsync(ComorbilidadesSeleccionadas);
            StateHasChanged();
        }

        public async Task RemoverComorbilidad(string comorbilidad)
        {
            ComorbilidadesSeleccionadas.Remove(comorbilidad);
            await ComorbilidadesSeleccionadasChanged.InvokeAsync(ComorbilidadesSeleccionadas);
            StateHasChanged();
        }

        // Método para los Toggle Buttons directos del UI
        public async Task SeleccionarTipoMesa(string tipo)
        {
            await TipoMesaSeleccionadoChanged.InvokeAsync(tipo);
        }
    }
}