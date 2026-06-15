using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Repository;

namespace ProyectoSantaMonica_Cesar.Service.EspecialidadService
{
    public class EspecialidadService : IEspecialidadService
    {
        private readonly EspecialidadRepository _repo;

        public EspecialidadService(EspecialidadRepository repo)
        {
            _repo = repo;
        }

        //Listar
        public async Task<List<Especialidad>> Listar()
            => await _repo.Listar();

        //Obtener por Id
        public async Task<Especialidad?> ObtenerPorId(int id)
            => await _repo.ObtenerPorId(id);

        //Registrar
        public async Task<string> Registrar(Especialidad especialidad)
        {
            if (string.IsNullOrEmpty(especialidad.Nombre))
                throw new Exception("El nombre es obligatorio");

            return await _repo.Registrar(especialidad);
        }

        //Actualizar
        public async Task<string> Actualizar(Especialidad especialidad)
        {
            if (especialidad.Id_Especialidad == 0)
                throw new Exception("ID inválido");

            return await _repo.Actualizar(especialidad);
        }

        //Eliminar
        public async Task<string> Eliminar(int id)
        {
            if (id == 0)
                throw new Exception("ID inválido");

            return await _repo.Eliminar(id);
        }
    }
}