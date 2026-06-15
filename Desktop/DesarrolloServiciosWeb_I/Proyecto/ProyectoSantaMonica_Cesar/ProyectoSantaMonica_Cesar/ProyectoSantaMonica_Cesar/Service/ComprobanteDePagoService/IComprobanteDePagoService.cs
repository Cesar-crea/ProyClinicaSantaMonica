using ProyectoSantaMonica_Cesar.Models;

namespace ProyectoSantaMonica_Cesar.Service.ComprobanteDePagoService
{
    public interface IComprobanteDePagoService
    {
        Task GuardarAsync(ComprobanteDePago c);
        Task<List<ComprobanteDePago>> ListarAsync();
        Task<List<ComprobanteDePago>> BuscarAsync(string dato);
        Task AnularAsync(int id);
        Task<ComprobanteDePago?> BuscarPorIdAsync(int id);

        Task<List<ComprobanteDePago>> BuscarPorPacienteAsync(string dato);
    }
}
