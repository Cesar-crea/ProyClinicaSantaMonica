using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Repository;

namespace ProyectoSantaMonica_Cesar.Service.ComprobanteDePagoService
{
    public class ComprobanteDePagoService : IComprobanteDePagoService
    {
        private readonly ComprobanteDePagoRepository repo;

        public ComprobanteDePagoService(ComprobanteDePagoRepository repo)
        {
            this.repo = repo;
        }

        public async Task GuardarAsync(ComprobanteDePago c)
        {
            await repo.InsertarAsync(c);
        }

        public async Task<List<ComprobanteDePago>> ListarAsync()
        {
            return await repo.ListarAsync();
        }

        public async Task<List<ComprobanteDePago>> BuscarAsync(string dato)
        {
            return await repo.BuscarAsync(dato);
        }

        public async Task AnularAsync(int id)
        {
            await repo.AnularAsync(id);
        }

        public async Task<ComprobanteDePago?> BuscarPorIdAsync(int id)
        {
            return await repo.BuscarPorIdAsync(id);
        }

        public async Task<List<ComprobanteDePago>> BuscarPorPacienteAsync(string dato)
        {
            return await repo.BuscarPorPacienteAsync(dato);
        }
    }
}
