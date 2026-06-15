namespace ProyectoSantaMonica_Cesar.Models.ViewModels
{
    public class HorariosViewModel
    {

        public List<Horario> Horarios { get; set; } = new();
        public List<Cita> Ocupadas { get; set; } = new();
    }
}
