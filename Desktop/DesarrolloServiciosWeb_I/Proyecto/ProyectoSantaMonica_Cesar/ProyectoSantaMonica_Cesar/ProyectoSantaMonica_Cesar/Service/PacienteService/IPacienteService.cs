using ProyectoSantaMonica_Cesar.Models;

namespace ProyectoSantaMonica_Cesar.Service.PacienteService
{
    public interface IPacienteService
    {
        void GuardarPaciente(Paciente paciente);
        List<Paciente> ListarTodosPaciente();
        void ActualizarPaciente(Paciente paciente);
        void EliminarPaciente(int id);
        Paciente BuscarPacientePorId(int id);
        List<Paciente> BuscarPacientePorNombre(string nombre);

        List<Paciente> BuscarGeneral(string texto);

    }
}
