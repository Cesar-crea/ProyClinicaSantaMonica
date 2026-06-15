using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Repository;

namespace ProyectoSantaMonica_Cesar.Service.PacienteService
{
    public class PacienteService : IPacienteService
    {
        private readonly PacienteRepository _repo;

        public PacienteService(PacienteRepository repo)
        {
            _repo = repo;
        }

        public void GuardarPaciente(Paciente paciente)
        {
            _repo.Insertar(paciente);
        }

        public List<Paciente> ListarTodosPaciente()
        {
            return _repo.Listar();
        }

        public void ActualizarPaciente(Paciente paciente)
        {
            _repo.Actualizar(paciente);
        }

        public void EliminarPaciente(int id)
        {
            _repo.Eliminar(id);
        }

        public Paciente BuscarPacientePorId(int id)
        {
            return _repo.BuscarPorId(id);
        }

        public List<Paciente> BuscarPacientePorNombre(string nombre)
        {
            return _repo.BuscarPorNombre(nombre);
        }

        public List<Paciente> BuscarGeneral(string texto)
        {
            return _repo.BuscarGeneral(texto);
        }

    }
    }
