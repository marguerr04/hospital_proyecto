using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using proyecto_hospital_version_1.Models;
using proyecto_hospital_version_1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Data._Legacy; // Añadir esto para usar Include y ToListAsync

namespace proyecto_hospital_version_1.Components.Shared
{
    public partial class Paso2RequerimientosCard : ComponentBase
    {
        [Inject] private IEspecialidadHospital EspecialidadHospitalService { get; set; } = default!;
        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;
        [Inject] private IDiagnosticoService DiagnosticoService { get; set; } = default!; // Aunque lo usaremos indirectamente, lo mantenemos por si acaso.
        [Inject] private HospitalDbContextLegacy HospitalDb { get; set; } = default!; // Inyectar DbContext para acceder directamente a las entidades

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
        private List<Diagnostico> _todosLosDiagnosticos = new List<Diagnostico>(); // Contendrá todos los diagnósticos cargados una vez.
        private List<Diagnostico> _diagnosticosFiltrados = new List<Diagnostico>(); // Contendrá los diagnósticos para mostrar en la lista sugerida.
        private List<string> _diagnosticosSugeridos = new List<string>(); // Nombres de los diagnósticos para el datalist.
        private List<string> _procedimientosAnadidos = new List<string>(); // Mantener esta, es para otro componente.

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Cargar especialidades
                _especialidadesDisponiblesBd = await EspecialidadHospitalService.GetEspecialidadesAsync() ?? new List<EspecialidadHospital>();
                Console.WriteLine($"Total especialidades cargadas: {_especialidadesDisponiblesBd.Count}");

                // Cargar TODOS los diagnósticos una única vez, incluyendo su relación con MapeoGes.
                // Esto es vital para poder filtrar correctamente.
                Console.WriteLine("Cargando todos los diagnósticos con sus mapeos GES...");
                _todosLosDiagnosticos = await HospitalDb.Diagnosticos
                                                        .Include(d => d.MapeosGes) // Asegúrate de que MapeosGes es una ICollection<MapeoGes> en tu modelo Diagnostico
                                                        .OrderBy(d => d.Nombre)
                                                        .ToListAsync();

                Console.WriteLine($"Total diagnósticos cargados (incluyendo no GES): {_todosLosDiagnosticos.Count}");
                foreach (var diag in _todosLosDiagnosticos.Take(5)) // Mostrar los primeros 5 para depuración
                {
                    Console.WriteLine($"- Diagnóstico: '{diag.Nombre}' (Id: {diag.Id}), ¿Tiene MapeosGes? {diag.MapeosGes != null && diag.MapeosGes.Any()}");
                    if (diag.MapeosGes != null)
                    {
                        foreach (var mapeo in diag.MapeosGes.Take(1)) // Mostrar solo el primer mapeo si existe
                        {
                            Console.WriteLine($"   - MapeoGes: DiagnosticoId={mapeo.DiagnosticoId}, PatologiaGesId={mapeo.PatologiaGesId}");
                        }
                    }
                }

                // Inicializar la lista de diagnósticos a mostrar (por defecto, todos si EsGes es false, o ya filtrados si EsGes es true al inicio)
                AplicarFiltroDiagnosticos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inicializando Paso 2: {ex.Message}");
                // Opcional: Mostrar alerta al usuario si hay un error crítico al cargar datos
                await JsRuntime.InvokeVoidAsync("alert", $"Error cargando datos iniciales del diagnóstico: {ex.Message}");
            }
        }

        /// <summary>
        /// Aplica el filtro GES a la lista de diagnósticos y actualiza las sugerencias.
        /// Se llama al inicio y cuando el checkbox EsGes cambia.
        /// </summary>
        private void AplicarFiltroDiagnosticos()
        {
            Console.WriteLine($"--- Aplicando filtro GES. EsGes: {EsGes} ---");
            List<Diagnostico> diagnosticosParaMostrar; // Variable local temporal

            if (EsGes)
            {
                // Filtrar _todosLosDiagnosticos para mostrar solo los que tienen al menos un MapeoGes
                diagnosticosParaMostrar = _todosLosDiagnosticos
                                            .Where(d => d.MapeosGes != null && d.MapeosGes.Any())
                                            .ToList();
                Console.WriteLine($"Diagnósticos filtrados (solo GES): {diagnosticosParaMostrar.Count}");
                foreach (var diag in diagnosticosParaMostrar.Take(5))
                {
                    Console.WriteLine($"- GES: '{diag.Nombre}'");
                }
            }
            else
            {
                // Mostrar todos los diagnósticos si EsGes no está marcado
                diagnosticosParaMostrar = _todosLosDiagnosticos;
                Console.WriteLine($"Diagnósticos sin filtrar (todos): {diagnosticosParaMostrar.Count}");
                foreach (var diag in diagnosticosParaMostrar.Take(5))
                {
                    Console.WriteLine($"- NO GES (todos): '{diag.Nombre}'");
                }
            }

            // Actualizar las listas del componente
            _diagnosticosFiltrados = diagnosticosParaMostrar;
            _diagnosticosSugeridos = _diagnosticosFiltrados.Select(d => d.Nombre).ToList();

            // Lógica para deseleccionar el diagnóstico principal si ya no está en la lista filtrada
            if (!string.IsNullOrEmpty(DiagnosticoPrincipal) && !_diagnosticosSugeridos.Contains(DiagnosticoPrincipal))
            {
                Console.WriteLine($"El diagnóstico '{DiagnosticoPrincipal}' ya no es válido para el filtro actual. Reiniciando campo.");
                DiagnosticoPrincipal = "";
                CodigoAsociado = "";
                // Notificar al componente padre que los valores han cambiado (si es necesario)
                DiagnosticoPrincipalChanged.InvokeAsync("");
                CodigoAsociadoChanged.InvokeAsync("");
            }
            StateHasChanged(); // Forzar el re-renderizado del componente para actualizar el datalist
            Console.WriteLine($"Datalist actualizado con {_diagnosticosSugeridos.Count} sugerencias para el estado EsGes={EsGes}.");
        }


        private void AgregarProcedimientoDummy()
        {
            _procedimientosAnadidos.Add($"Procedimiento {(_procedimientosAnadidos.Count + 1)}");
        }

        // --- MÉTODOS DE MANEJO DE CAMBIOS (Event Handlers) ---
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

        // Este método se dispara cuando el checkbox "EsGes" cambia de estado
        private async Task OnEsGesChanged(ChangeEventArgs e)
        {
            if (e.Value is bool valor)
            {
                // NOTA: Directamente actualizamos el parámetro aquí, Blazor lo propagará.
                // Si este componente fuera el único lugar donde se usa EsGes, podríamos usar una variable de campo privada.
                // Para simplificar, asumimos que el padre actualizará el parámetro y Blazor re-renderizará,
                // pero si no, también podríamos setear this.EsGes = valor; aquí.
                await EsGesChanged.InvokeAsync(valor);
                AplicarFiltroDiagnosticos(); // Volver a aplicar el filtro con el nuevo estado de EsGes
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
                var diagnosticoEncontrado = _diagnosticosFiltrados // Buscar en la lista ya filtrada (_diagnosticosFiltrados)
                    .FirstOrDefault(d => d.Nombre.Equals(nuevoDiagnostico, StringComparison.OrdinalIgnoreCase));

                if (diagnosticoEncontrado != null)
                {
                    Console.WriteLine($"Código CIE asociado encontrado: {diagnosticoEncontrado.CodigoCie}");
                    await CodigoAsociadoChanged.InvokeAsync(diagnosticoEncontrado.CodigoCie ?? "");
                }
                else
                {
                    Console.WriteLine("Diagnóstico no encontrado en la lista actual de sugerencias. Limpiando código.");
                    await CodigoAsociadoChanged.InvokeAsync(""); // Limpiar si no se encuentra
                }
            }
            else
            {
                Console.WriteLine("Campo de diagnóstico vacío. Limpiando código.");
                await CodigoAsociadoChanged.InvokeAsync(""); // Limpiar si el campo está vacío
            }
        }
    }
}