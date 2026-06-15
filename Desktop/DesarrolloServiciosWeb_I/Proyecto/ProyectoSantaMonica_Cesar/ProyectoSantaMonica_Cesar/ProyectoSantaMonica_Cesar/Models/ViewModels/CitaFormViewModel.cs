namespace ProyectoSantaMonica_Cesar.Models.ViewModels
{
    public class CitaFormViewModel
    {
        public Cita Cita { get; set; } = new Cita();

        public List<Paciente> Pacientes { get; set; } = new();
        public List<Medico> Medicos { get; set; } = new();

        public HorariosViewModel Horarios { get; set; } = new();
    }
}
