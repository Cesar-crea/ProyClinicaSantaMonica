using ProyectoSantaMonica_Cesar.Models;

namespace ProyectoSantaMonica_Cesar.Service.EspecialidadService
{
    public interface IEspecialidadService
    {
        Task<List<Especialidad>> Listar();

        Task<Especialidad?> ObtenerPorId(int id);

        Task<string> Registrar(Especialidad especialidad);

        Task<string> Actualizar(Especialidad especialidad);

        Task<string> Eliminar(int id);
    }
}
