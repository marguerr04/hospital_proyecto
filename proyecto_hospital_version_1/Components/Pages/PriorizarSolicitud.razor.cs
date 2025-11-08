using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using proyecto_hospital_version_1.Data._Legacy; // Asegúrate que los 'using' sean correctos
using proyecto_hospital_version_1.Shared; // Asegúrate que los 'using' sean correctos
using MudBlazor; // Tienes 'using MudBlazor' en el razor
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace proyecto_hospital_version_1.Components.Pages
{

    public partial class PriorizarSolicitudes : ComponentBase
    {
        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        private HospitalDbContextLegacy HospitalDb { get; set; }

        // --- LÓGICA DE LISTA Y PAGINACIÓN ---
        private List<SolicitudQuirurgica> _solicitudesPendientes;
        private List<SolicitudQuirurgica> _solicitudesPriorizadas;

        private List<SolicitudQuirurgica> _solicitudesPendientesPagina;
        private List<SolicitudQuirurgica> _solicitudesPriorizadasPagina;
        private int _paginaActualPendientes = 1;
        private int _paginaActualPriorizadas = 1;
        private int _elementosPorPagina = 5;
        private int _totalPaginasPendientes;
        private int _totalPaginasPriorizadas;

        // Este método se llama cuando la página carga
        protected override async Task OnInitializedAsync()
        {
            await CargarSolicitudes();
        }

        private async Task CargarSolicitudes()
        {
            try
            {
                _solicitudesPendientes = await HospitalDb.SolicitudesQuirurgicas
                    .Include(s => s.Paciente)
                    .Where(s => s.Estado == "Pendiente" || string.IsNullOrEmpty(s.Estado))
                    .OrderByDescending(s => s.FechaCreacion)
                    .AsNoTracking()
                    .ToListAsync();

                _solicitudesPriorizadas = await HospitalDb.SolicitudesQuirurgicas
                    .Include(s => s.Paciente)
                    .Where(s => s.Estado == "Priorizada" || s.Estado == "Priorizada (Manual)")
                    .OrderBy(s => s.Prioridad)
                    .ThenByDescending(s => s.FechaPriorizacion)
                    .AsNoTracking()
                    .ToListAsync();

                CalcularPaginacion();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar solicitudes: {ex.Message}");
                _solicitudesPendientes = new List<SolicitudQuirurgica>();
                _solicitudesPriorizadas = new List<SolicitudQuirurgica>();
                CalcularPaginacion();
            }
        }

        private void CalcularPaginacion()
        {
            // PAGINACIÓN PARA SOLICITUDES PENDIENTES
            _totalPaginasPendientes = (int)Math.Ceiling(_solicitudesPendientes.Count / (double)_elementosPorPagina);
            if (_totalPaginasPendientes == 0) _totalPaginasPendientes = 1;

            _solicitudesPendientesPagina = _solicitudesPendientes
                .Skip((_paginaActualPendientes - 1) * _elementosPorPagina)
                .Take(_elementosPorPagina)
                .ToList();

            // PAGINACIÓN PARA SOLICITUDES PRIORIZADAS
            _totalPaginasPriorizadas = (int)Math.Ceiling(_solicitudesPriorizadas.Count / (double)_elementosPorPagina);
            if (_totalPaginasPriorizadas == 0) _totalPaginasPriorizadas = 1;

            _solicitudesPriorizadasPagina = _solicitudesPriorizadas
                .Skip((_paginaActualPriorizadas - 1) * _elementosPorPagina)
                .Take(_elementosPorPagina)
                .ToList();
        }

        private void CambiarPaginaPendientes(int nuevaPagina)
        {
            if (nuevaPagina >= 1 && nuevaPagina <= _totalPaginasPendientes)
            {
                _paginaActualPendientes = nuevaPagina;
                CalcularPaginacion();
                StateHasChanged();
            }
        }

        private void CambiarPaginaPriorizadas(int nuevaPagina)
        {
            if (nuevaPagina >= 1 && nuevaPagina <= _totalPaginasPriorizadas)
            {
                _paginaActualPriorizadas = nuevaPagina;
                CalcularPaginacion();
                StateHasChanged();
            }
        }

        // --- MÉTODOS DE FILTRO Y NAVEGACIÓN ---

        // ¡¡IMPORTANTE!! Este método ahora solo NAVEGA a la página de detalle
        private void VerDetalle(int id)
        {
            Navigation.NavigateTo($"/priorizar-solicitudes/{id}");
        }

        private async Task FiltrarPorPaciente(PacienteHospital? paciente)
        {
            if (paciente == null)
            {
                await CargarSolicitudes();
                return;
            }

            try
            {
                _solicitudesPendientes = await HospitalDb.SolicitudesQuirurgicas
                    .Include(s => s.Paciente)
                    .Where(s => s.Paciente.id == paciente.id && (s.Estado == "Pendiente" || string.IsNullOrEmpty(s.Estado)))
                    .OrderByDescending(s => s.FechaCreacion)
                    .AsNoTracking()
                    .ToListAsync();

                _solicitudesPriorizadas = await HospitalDb.SolicitudesQuirurgicas
                    .Include(s => s.Paciente)
                    .Where(s => s.Paciente.id == paciente.id && (s.Estado == "Priorizada" || s.Estado == "Priorizada (Manual)"))
                    .OrderByDescending(s => s.FechaPriorizacion)
                    .AsNoTracking()
                    .ToListAsync();

                _paginaActualPendientes = 1;
                _paginaActualPriorizadas = 1;
                CalcularPaginacion();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al filtrar por paciente: {ex.Message}");
            }
        }
    }
}