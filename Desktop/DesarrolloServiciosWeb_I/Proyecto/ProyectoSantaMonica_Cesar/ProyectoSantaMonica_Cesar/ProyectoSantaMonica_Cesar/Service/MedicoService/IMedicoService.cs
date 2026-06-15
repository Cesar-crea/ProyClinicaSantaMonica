using ProyectoSantaMonica_Cesar.Models;

namespace ProyectoSantaMonica_Cesar.Service.MedicoService
{
    public interface IMedicoService
    {
        Task<List<Medico>> ListarTodosAsync();
        Task GuardarAsync(Medico medico);
        Task ActualizarAsync(Medico medico);
        Task EliminarAsync(int id);
        Task<List<Medico>> BuscarPorNombreAsync(string nombre);
        Task<Medico?> BuscarPorIdAsync(int id);
        Task<List<Horario>> ObtenerHorariosPorMedicoAsync(int idMedico);

        Task<List<Medico>> BuscarGeneralAsync(string texto);

    }
}
