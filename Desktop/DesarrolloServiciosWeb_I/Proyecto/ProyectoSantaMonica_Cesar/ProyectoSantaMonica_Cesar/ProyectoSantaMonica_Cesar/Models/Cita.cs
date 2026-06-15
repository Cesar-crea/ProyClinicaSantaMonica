using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ProyectoSantaMonica_Cesar.Models.enums;

namespace ProyectoSantaMonica_Cesar.Models
{
    public class Cita
    {
        [Key]
        [Column("Id_Cita")]
        public int Id_Cita { get; set; }

        [Required(ErrorMessage = "El médico es obligatorio")]
        [Column("Id_Medico")]
        public int Id_Medico { get; set; }

        [Required(ErrorMessage = "El paciente es obligatorio")]
        [Column("Id_Paciente")]
        public int Id_Paciente { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        [Column("Id_Usuario")]
        public long Id_Usuario { get; set; }


        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        [Column("Fecha")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La hora es obligatoria")]
        [DataType(DataType.Time)]
        [Column("Hora")]
        public TimeSpan Hora { get; set; }

        [StringLength(255, ErrorMessage = "Máximo 255 caracteres")]
        [Column("Motivo")]
        public string? Motivo { get; set; }


        [Required]
        [Column("Estado", TypeName = "nvarchar(20)")]
        public EstadoCita Estado { get; set; } = EstadoCita.PENDIENTE;

        [ForeignKey("Id_Medico")]
        public Medico? Medico { get; set; }

        [ForeignKey("Id_Paciente")]
        public Paciente? Paciente { get; set; }

        [ForeignKey("Id_Usuario")]
        public Usuario? Usuario
        {get; set;}

    }
}
