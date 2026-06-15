using ProyectoSantaMonica_Cesar.Models.enums;
using ProyectoSantaMonica_Cesar.Models;

namespace ProyectoSantaMonica_Cesar.Service.HorarioService
{
    public interface IHorarioService
    {
        Task<List<Horario>> ListarPorMedicoAsync(int idMedico);
        Task<List<Horario>> ListarPorMedicoDiaAsync(int idMedico, DiaSemana dia);
        Task<Horario?> ObtenerPorIdAsync(int id);

        Task<string> RegistrarAsync(Horario horario);
        Task<string> ActualizarAsync(Horario horario);
        Task<string> EliminarAsync(int id);
    }
}
