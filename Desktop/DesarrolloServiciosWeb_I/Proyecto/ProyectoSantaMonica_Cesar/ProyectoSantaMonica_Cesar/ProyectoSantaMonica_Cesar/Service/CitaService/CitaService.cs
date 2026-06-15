using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Models.enums;
using ProyectoSantaMonica_Cesar.Repository;


namespace ProyectoSantaMonica_Cesar.Service.CitaService
{
    public class CitaService : ICitaService
    {
        private readonly CitaRepository _repo;

        public CitaService(CitaRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Cita>> ListarAsync()
        {
            await _repo.ActualizarEstadosAutomaticosAsync(); // 🔥 clave
            return await _repo.ListarAsync();
        }

        public async Task<List<Cita>> BuscarPorPacienteAsync(string texto)
            => await _repo.BuscarPorPacienteAsync(texto);

        public async Task<string> RegistrarAsync(Cita c)
        {
            Validar(c);
            return await _repo.RegistrarAsync(c);
        }

        public async Task<string> ActualizarAsync(Cita c)
        {
            Validar(c);
            return await _repo.ActualizarAsync(c);
        }

        public async Task<string> EliminarAsync(int id)
            => await _repo.EliminarAsync(id);

        public async Task<Cita?> ObtenerPorIdAsync(int id)
            => await _repo.ObtenerPorIdAsync(id);

        public async Task<List<Cita>> HorariosOcupadosAsync(int idMedico, DateTime fecha)
        {
            return await _repo.HorariosOcupadosAsync(idMedico, fecha);
        }
        public async Task<List<Cita>> ListarPorEstadoAsync(EstadoCita estado)
            => await _repo.ListarPorEstadoAsync(estado.ToString());

        public async Task<List<Cita>> PendientesPorPacienteAsync(string dato)
            => await _repo.PendientesPorPacienteAsync(dato);

        public async Task<string> ActualizarEstadoAsync(int id, EstadoCita estado)
            => await _repo.ActualizarEstadoAsync(id, estado);

        public async Task ActualizarEstadosAutomaticosAsync()
            => await _repo.ActualizarEstadosAutomaticosAsync();

        private void Validar(Cita c)
        {
            if (c.Fecha < DateTime.Now.Date)
                throw new Exception("Fecha inválida");

            if (c.Hora == default)
                throw new Exception("Hora inválida");
        }

        public async Task<bool> MedicoAtiendeEnHorarioAsync(
    int idMedico,
    DateTime fecha,
    TimeSpan hora)
        {
            return await _repo.MedicoAtiendeEnHorarioAsync(
                idMedico,
                fecha,
                hora);
        }

    }
}