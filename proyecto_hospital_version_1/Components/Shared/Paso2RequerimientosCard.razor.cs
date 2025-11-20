using Hospital.Api.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using proyecto_hospital_version_1.Data._Legacy;
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

        [Parameter] public PacienteDto? Paciente { get; set; }
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
        [Parameter] public List<ProcSelVM> ProcedimientosSecundarios { get; set; } = new();
        [Parameter] public EventCallback<List<ProcSelVM>> ProcedimientosSecundariosChanged { get; set; }

        // Variables locales - CAMBIAR a DiagnosticoDto
        private List<EspecialidadHospital> _especialidadesDisponiblesBd = new();
        private List<DiagnosticoDto> _todosLosDiagnosticos = new(); // Cambio aquí
        private List<DiagnosticoDto> _diagnosticosFiltrados = new(); // Cambio aquí
        private List<string> _diagnosticosSugeridos = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Cargar especialidades
                _especialidadesDisponiblesBd = await EspecialidadHospitalService.GetEspecialidadesAsync() ?? new List<EspecialidadHospital>();
                Console.WriteLine($"Total especialidades cargadas: {_especialidadesDisponiblesBd.Count}");

                // Cargar TODOS los diagnósticos desde la API - CAMBIO PRINCIPAL
                Console.WriteLine("Cargando todos los diagnósticos desde la API...");
                _todosLosDiagnosticos = await DiagnosticoService.GetDiagnosticosAsync();

                Console.WriteLine($"Total diagnósticos cargados desde API: {_todosLosDiagnosticos.Count}");
                foreach (var diag in _todosLosDiagnosticos.Take(5))
                {
                    Console.WriteLine($"- Diagnóstico: '{diag.Nombre}' (Id: {diag.Id}), EsGes: {diag.EsGes}");
                }

                // Inicializar la lista de diagnósticos a mostrar
                await AplicarFiltroDiagnosticos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inicializando Paso 2: {ex.Message}");
                await JsRuntime.InvokeVoidAsync("alert", $"Error cargando datos iniciales del diagnóstico: {ex.Message}");
            }
        }
        // logica del GES
        private async Task AplicarFiltroDiagnosticos()
        {
            Console.WriteLine($"--- Aplicando filtro GES. EsGes: {EsGes} ---");
            List<DiagnosticoDto> diagnosticosParaMostrar;

            if (EsGes)
            {
                // Opción A: Usar solo los que ya vienen con EsGes=true
                diagnosticosParaMostrar = _todosLosDiagnosticos
                    .Where(d => d.EsGes)
                    .ToList();



                Console.WriteLine($"Diagnósticos filtrados (solo GES): {diagnosticosParaMostrar.Count}");
            }
            else
            {
                diagnosticosParaMostrar = _todosLosDiagnosticos;
                Console.WriteLine($"Diagnósticos sin filtrar (todos): {diagnosticosParaMostrar.Count}");
            }

            _diagnosticosFiltrados = diagnosticosParaMostrar;

            // Dugerencias para diasnisticodto
            _diagnosticosSugeridos = _diagnosticosFiltrados
                .Select(d => d.Nombre ?? string.Empty)
                .Distinct()
                .ToList();

            //  No seleccioanr diagnostico principal
            if (!string.IsNullOrEmpty(DiagnosticoPrincipal) &&
                !_diagnosticosSugeridos.Contains(DiagnosticoPrincipal, StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine($"El diagnóstico '{DiagnosticoPrincipal}' ya no es válido para el filtro actual. Reiniciando campo.");
                DiagnosticoPrincipal = "";
                CodigoAsociado = "";
                await DiagnosticoPrincipalChanged.InvokeAsync("");
                await CodigoAsociadoChanged.InvokeAsync("");
            }

            await InvokeAsync(StateHasChanged);
            Console.WriteLine($"Datalist actualizado con {_diagnosticosSugeridos.Count} sugerencias para el estado EsGes={EsGes}.");
        }

        // --- Handlers para Procedimientos secundarios (sin cambios) ---
        private Task OnAddProc(ProcSelVM vm) => ProcedimientosSecundariosChanged.InvokeAsync(ProcedimientosSecundarios);
        private Task OnEditProc(ProcSelVM vm) => ProcedimientosSecundariosChanged.InvokeAsync(ProcedimientosSecundarios);
        private Task OnDeleteProc(ProcSelVM vm) => ProcedimientosSecundariosChanged.InvokeAsync(ProcedimientosSecundarios);

  
        private async Task OnPesoChanged(ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out decimal peso))
                await PesoChanged.InvokeAsync(peso);
            else
                await PesoChanged.InvokeAsync(null);
        }

        private async Task OnTallaChanged(ChangeEventArgs e)
        {
            if (decimal.TryParse(e.Value?.ToString(), out decimal talla))
                await TallaChanged.InvokeAsync(talla);
            else
                await TallaChanged.InvokeAsync(null);
        }

        private async Task OnEsGesChanged(ChangeEventArgs e)
        {
            if (e.Value is bool valor)
            {
                await EsGesChanged.InvokeAsync(valor);
                await AplicarFiltroDiagnosticos(); // Cambio: hacerlo async
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

        // Este método se dispara cuando el usuario escribe o selecciona un diagnóstico
        private async Task OnDiagnosticoInput(ChangeEventArgs e)
        {
            var nuevoDiagnostico = e.Value?.ToString() ?? "";
            Console.WriteLine($"Diagnóstico ingresado/seleccionado: {nuevoDiagnostico}");

            // 1. Actualizar el parámetro DiagnosticoPrincipal
            await DiagnosticoPrincipalChanged.InvokeAsync(nuevoDiagnostico);

            // 2. Buscar el diagnóstico en la lista FILTRADA y actualizar el código CIE
            if (!string.IsNullOrWhiteSpace(nuevoDiagnostico))
            {
                var diagnosticoEncontrado = _diagnosticosFiltrados
                    .FirstOrDefault(d => d.Nombre.Equals(nuevoDiagnostico, StringComparison.OrdinalIgnoreCase));

                if (diagnosticoEncontrado != null)
                {
                    Console.WriteLine($"Código CIE asociado encontrado: {diagnosticoEncontrado.CodigoCie}");
                    await CodigoAsociadoChanged.InvokeAsync(diagnosticoEncontrado.CodigoCie ?? "");
                }
                else
                {
                    Console.WriteLine("Diagnóstico no encontrado en la lista actual de sugerencias. Limpiando código.");
                    await CodigoAsociadoChanged.InvokeAsync("");
                }
            }
            else
            {
                Console.WriteLine("Campo de diagnóstico vacío. Limpiando código.");
                await CodigoAsociadoChanged.InvokeAsync("");
            }
        }
    }
}