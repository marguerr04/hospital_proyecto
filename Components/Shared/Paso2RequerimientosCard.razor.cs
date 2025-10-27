// Namespaces que estaban en los @using
using proyecto_hospital_version_1.Data.Hospital;
using proyecto_hospital_version_1.Models;
using proyecto_hospital_version_1.Services;

// Namespaces de Blazor necesarios
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace proyecto_hospital_version_1.Components.Shared
{
    // Debe ser 'public partial' y tener el mismo nombre que el .razor
    public partial class Paso2RequerimientosCard : ComponentBase
    {
        // Los @inject se convierten en propiedades [Inject]
        [Inject]
        private IEspecialidadHospital EspecialidadHospitalService { get; set; } = default!;

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        private IDiagnosticoService DiagnosticoService { get; set; } = default!;


        // --- Todo lo que estaba en el @code ---

        [Parameter]
        public PacienteHospital? Paciente { get; set; }

        private List<EspecialidadHospital> _especialidadesDisponiblesBd = new List<EspecialidadHospital>();
        private string _codigoAsociado = string.Empty;
        private List<Diagnostico> _todosLosDiagnosticos = new List<Diagnostico>();


        // para el diagnostico principal
        [Parameter]
        public string DiagnosticoPrincipal { get; set; } = "";
        [Parameter]
        public EventCallback<string> DiagnosticoPrincipalChanged { get; set; }

        [Parameter]
        public bool EsGes { get; set; }
        [Parameter]
        public EventCallback<bool> EsGesChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _especialidadesDisponiblesBd = await EspecialidadHospitalService.GetEspecialidadesAsync();
            _todosLosDiagnosticos = await DiagnosticoService.GetDiagnosticosAsync();
            _diagnosticosSugeridos = _todosLosDiagnosticos.Select(d => d.Nombre).ToList();
        }

        private decimal _peso = 0;
        private decimal _talla = 0;
        private decimal _imc => (_talla > 0) ? _peso / (_talla * _talla) : 0;
        private string _codigoCie = string.Empty;

        private List<string> _diagnosticosSugeridos = new List<string>();

        // NOTA: Quité _diagnosticoPrincipal y _esGes privados porque eran redundantes.
        // Los parámetros [Parameter] ya cumplen esa función.

        private string _especialidadOrigen = string.Empty;
        private string _especialidadDestino = string.Empty;

        private List<string> _procedimientosAnadidos = new List<string>();

        protected override void OnParametersSet()
        {
            // Ya no es necesario asignar los parámetros a campos privados aquí
        }

        private void AgregarProcedimientoDummy()
        {
            _procedimientosAnadidos.Add($"Procedimiento {(_procedimientosAnadidos.Count + 1)}");
        }

        private async Task OnDiagnosticoPrincipalChanged(string value)
        {
            DiagnosticoPrincipal = value ?? "";

            var diagnosticoEncontrado = _todosLosDiagnosticos
                .FirstOrDefault(d => d.Nombre.Equals(value, StringComparison.OrdinalIgnoreCase));

            _codigoAsociado = diagnosticoEncontrado?.CodigoCie ?? "";

            await DiagnosticoPrincipalChanged.InvokeAsync(DiagnosticoPrincipal);
            StateHasChanged();
        }

        // NOTA: Quité el método OnEsGesChanged(bool value) porque no se estaba usando.

        private async Task OnEsGesCheckboxChanged(ChangeEventArgs e)
        {
            // Es más seguro comprobar el tipo de e.Value
            if (e.Value is bool valorCheckbox)
            {
                EsGes = valorCheckbox;
                await EsGesChanged.InvokeAsync(EsGes);
                StateHasChanged(); // StateHasChanged es necesario aquí
            }
        }
    }
}