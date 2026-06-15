using ProyectoSantaMonica_Cesar.Models.enums;
using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Repository;

namespace ProyectoSantaMonica_Cesar.Service.HorarioService
{
    public class HorarioServiceImpl : IHorarioService
    {
        private readonly HorarioRepository _repo;

        public HorarioServiceImpl(HorarioRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Horario>> ListarPorMedicoAsync(int idMedico)
        {
            return await _repo.ListarPorMedico(idMedico);
        }

        public async Task<List<Horario>> ListarPorMedicoDiaAsync(int idMedico, DiaSemana dia)
        {
            return await _repo.ListarPorMedicoDia(idMedico, dia);
        }

        public async Task<Horario?> ObtenerPorIdAsync(int id)
        {
            return await _repo.ObtenerPorId(id);
        }

        public async Task<string> RegistrarAsync(Horario h)
        {
            //  VALIDACIONES BÁSICAS
            if (h.Id_Medico == 0)
                throw new Exception("Debe seleccionar un médico");

            if (h.Horario_Entrada == default || h.Horario_Salida == default)
                throw new Exception("Debe ingresar horas válidas");

            ValidarHorario(h);

            // VALIDAR SOLAPAMIENTO
            var lista = await _repo.ListarPorMedico(h.Id_Medico);

            bool solapado = lista.Any(x =>
                x.Dia_Semana == h.Dia_Semana &&
                h.Horario_Entrada < x.Horario_Salida &&
                h.Horario_Salida > x.Horario_Entrada
            );

            if (solapado)
                throw new Exception("Horario se solapa con otro existente");

            //  REGISTRAR
            var resultado = await _repo.Registrar(h);

            if (resultado != "OK")
                throw new Exception("Error al registrar el horario");

            return resultado;
        }

        public async Task<string> ActualizarAsync(Horario h)
        {
            ValidarHorario(h);
            return await _repo.Actualizar(h);
        }

        public async Task<string> EliminarAsync(int id)
        {
            return await _repo.Eliminar(id);
        }

        private void ValidarHorario(Horario h)
        {
            if (!h.Horario_Entrada.HasValue || !h.Horario_Salida.HasValue)
                throw new Exception("Debe ingresar horas válidas");

            if (h.Horario_Salida <= h.Horario_Entrada)
                throw new Exception("La hora de salida debe ser mayor");
        }
    }
}
