namespace ProyectoSantaMonica_Cesar.Models.enums
{
    public static class DIaSemanaHelper
    {
        public static DiaSemana FromDateTime(DateTime fecha)
        {
            return fecha.DayOfWeek switch
            {
                DayOfWeek.Monday => DiaSemana.LUNES,
                DayOfWeek.Tuesday => DiaSemana.MARTES,
                DayOfWeek.Wednesday => DiaSemana.MIERCOLES,
                DayOfWeek.Thursday => DiaSemana.JUEVES,
                DayOfWeek.Friday => DiaSemana.VIERNES,
                DayOfWeek.Saturday => DiaSemana.SABADO,
                DayOfWeek.Sunday => DiaSemana.DOMINGO,
                _ => throw new ArgumentException("Fecha inválida")
            };
        }
    }
}
