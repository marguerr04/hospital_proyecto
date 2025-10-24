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

        public async Task<int> ObtenerPercentil75Async()
        {
            return await _context.Procedimientos.CountAsync(); // Ejemplo
        }

        public async Task<int> ObtenerReduccionAsync()
        {
            return 230; // Aquí puedes traer el valor real desde BD
        }

        public async Task<int> ObtenerPendientesAsync()
        {
            return 268; // Igual que arriba
        }

        public async Task<double> ObtenerPorcentajeContactoAsync()
        {
            return 66.4;
        }
    }
}
