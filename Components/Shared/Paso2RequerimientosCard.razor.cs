using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using proyecto_hospital_version_1.Data.Hospital;
using proyecto_hospital_version_1.Models;
using proyecto_hospital_version_1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace proyecto_hospital_version_1.Components.Shared
{
    public partial class Paso2RequerimientosCard : ComponentBase
    {
        [Inject] private IEspecialidadHospital EspecialidadHospitalService { get; set; } = default!;
        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;
        [Inject] private IDiagnosticoService DiagnosticoService { get; set; } = default!;

        // --- PARÁMETROS ---
        [Parameter] public PacienteHospital? Paciente { get; set; }
        [Parameter] public string DiagnosticoPrincipal { get; set; } = "";
        [Parameter] public EventCallback<string> DiagnosticoPrincipalChanged { get; set; }
        [Parameter] public bool EsGes { get; set; }
        [Parameter] public EventCallback<bool> EsGesChanged { get; set; }
        [Parameter] public decimal? Peso { get; set; }
        [Parameter] public EventCallback<decimal?> PesoChanged { get; set; }
        [Parameter] public decimal? Talla { get; set; }
        [Parameter] public EventCallback<decimal?> TallaChanged { get; set; }
        [Parameter] public decimal? IMC { get; set; }
        [Parameter] public string CodigoAsociado { get; set; } = string.Empty;
        [Parameter] public EventCallback<string> CodigoAsociadoChanged { get; set; }
        [Parameter] public string EspecialidadOrigen { get; set; } = string.Empty;
        [Parameter] public EventCallback<string> EspecialidadOrigenChanged { get; set; }
        [Parameter] public string EspecialidadDestino { get; set; } = string.Empty;
        [Parameter] public EventCallback<string> EspecialidadDestinoChanged { get; set; }

        // Variables locales
        private List<EspecialidadHospital> _especialidadesDisponiblesBd = new List<EspecialidadHospital>();
        private List<Diagnostico> _todosLosDiagnosticos = new List<Diagnostico>();
        private List<string> _diagnosticosSugeridos = new List<string>();
        private List<string> _procedimientosAnadidos = new List<string>();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _especialidadesDisponiblesBd = await EspecialidadHospitalService.GetEspecialidadesAsync() ?? new List<EspecialidadHospital>();
                _todosLosDiagnosticos = await DiagnosticoService.GetDiagnosticosAsync() ?? new List<Diagnostico>();
                _diagnosticosSugeridos = _todosLosDiagnosticos.Select(d => d.Nombre).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inicializando Paso 2: {ex.Message}");
            }
        }

        private void AgregarProcedimientoDummy()
        {
            _procedimientosAnadidos.Add($"Procedimiento {(_procedimientosAnadidos.Count + 1)}");
        }

        // --- MÉTODOS SIMPLIFICADOS ---
        private async Task OnPesoChanged(ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out decimal peso))
            {
                await PesoChanged.InvokeAsync(peso);
            }
            else
            {
                await PesoChanged.InvokeAsync(null);
            }
        }

        private async Task OnTallaChanged(ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out decimal talla))
            {
                await TallaChanged.InvokeAsync(talla);
            }
            else
            {
                await TallaChanged.InvokeAsync(null);
            }
        }

        private async Task OnEsGesChanged(ChangeEventArgs e)
        {
            if (e.Value is bool valor)
            {
                await EsGesChanged.InvokeAsync(valor);
            }
        }

        private async Task OnEspecialidadOrigenChanged(ChangeEventArgs e)
        {
            await EspecialidadOrigenChanged.InvokeAsync(e.Value?.ToString() ?? "");
        }

        private async Task OnEspecialidadDestinoChanged(ChangeEventArgs e)
        {
            await EspecialidadDestinoChanged.InvokeAsync(e.Value?.ToString() ?? "");
        }

        private async Task OnDiagnosticoInput(ChangeEventArgs e)
        {
            var nuevoDiagnostico = e.Value?.ToString() ?? "";

            // Log para debug
            Console.WriteLine($"Diagnóstico seleccionado: {nuevoDiagnostico}");
            Console.WriteLine($"Total diagnósticos en memoria: {_todosLosDiagnosticos.Count}");

            // 1. Actualizar el diagnóstico
            await DiagnosticoPrincipalChanged.InvokeAsync(nuevoDiagnostico);

            // 2. Buscar y actualizar el código CIE
            if (!string.IsNullOrWhiteSpace(nuevoDiagnostico))
            {
                var diagnostico = _todosLosDiagnosticos
                    .FirstOrDefault(d => d.Nombre.Equals(nuevoDiagnostico, StringComparison.OrdinalIgnoreCase));

                if (diagnostico != null)
                {
                    Console.WriteLine($"Código CIE encontrado: {diagnostico.CodigoCie}");
                    await CodigoAsociadoChanged.InvokeAsync(diagnostico.CodigoCie ?? "");
                }
                else
                {
                    Console.WriteLine("No se encontró el diagnóstico en la lista");
                    await CodigoAsociadoChanged.InvokeAsync("");
                }
            }
            else
            {
                await CodigoAsociadoChanged.InvokeAsync("");
            }
        }
    }
}