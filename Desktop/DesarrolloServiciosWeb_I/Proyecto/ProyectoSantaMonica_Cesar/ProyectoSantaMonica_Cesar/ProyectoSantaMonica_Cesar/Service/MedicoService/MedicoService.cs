using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Repository;

namespace ProyectoSantaMonica_Cesar.Service.MedicoService
{
    public class MedicoService :IMedicoService
    {
        private readonly MedicoRepository _repo;

        public MedicoService(MedicoRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Medico>> ListarTodosAsync()
            => await _repo.ListarAsync();

        public async Task GuardarAsync(Medico medico)
            => await _repo.InsertarAsync(medico);

        public async Task ActualizarAsync(Medico medico)
            => await _repo.ActualizarAsync(medico);

        public async Task EliminarAsync(int id)
            => await _repo.EliminarAsync(id);

        public async Task<List<Medico>> BuscarPorNombreAsync(string nombre)
            => await _repo.BuscarPorNombreAsync(nombre);

        public async Task<Medico?> BuscarPorIdAsync(int id)
            => await _repo.BuscarPorIdAsync(id);

        public async Task<List<Horario>> ObtenerHorariosPorMedicoAsync(int idMedico)
        {
             return await _repo.ObtenerHorariosPorMedicoAsync(idMedico);
        }

        public async Task<List<Medico>> BuscarGeneralAsync(string texto)
        {
            return await _repo.BuscarGeneralAsync(texto);
        }

    }
}
