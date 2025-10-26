using Microsoft.EntityFrameworkCore;

namespace proyecto_hospital_version_1.Data
{
    public class DashboardService
    {
        private readonly HospitalDbContext _context;

        public DashboardService(HospitalDbContext context)
        {
            _context = context;
        }

        // Percentil 75: valor único
        public async Task<int> ObtenerPercentil75Async()
        {
            // Aquí podrías calcularlo con datos reales desde la BD
            return await Task.FromResult(75);
        }

        // Reducción: valor único
        public async Task<int> ObtenerReduccionAsync()
        {
            return await Task.FromResult(25);
        }

        // Pendientes: valor único
        public async Task<int> ObtenerPendientesAsync()
        {
            return await Task.FromResult(20);
        }

        // Contactabilidad: Diccionario para gráfico donut
        public async Task<Dictionary<string, double>> ObtenerPorcentajeContactoAsync()
        {
            var data = new Dictionary<string, double>
            {
                { "Contactado", 66.4 },
                { "En proceso", 20.0 },
                { "No Contactado", 13.6 }
            };
            return await Task.FromResult(data);
        }

        // Procedimientos por tipo: Diccionario para gráfico barras
        public async Task<Dictionary<string, int>> ObtenerProcedimientosPorTipoAsync()
        {
            var data = new Dictionary<string, int>
            {
                { "Cirugía Menor", 10 },
                { "Cirugía Mayor", 5 },
                { "Endoscopía", 7 }
            };
            return await Task.FromResult(data);
        }
    }
}
