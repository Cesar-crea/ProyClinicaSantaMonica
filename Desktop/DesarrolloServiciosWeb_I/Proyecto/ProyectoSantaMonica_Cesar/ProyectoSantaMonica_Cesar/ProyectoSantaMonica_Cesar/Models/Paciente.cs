using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProyectoSantaMonica_Cesar.Models
{
    public class Paciente
    {
     
        [Key]
        [Column("Id_Paciente")]
        public int Id_Paciente { get; set; }

      
        [Required(ErrorMessage = "Los nombres son obligatorios")]
        [StringLength(100)]
        [Column("Nombres")]
        public string Nombres { get; set; }

       
        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(50)]
        [Column("Apellidos")]
        public string Apellidos { get; set; }

    
        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 dígitos")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe contener solo números")]
        [Column("Dni")]
        public string Dni { get; set; }

      
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        [Column("Fecha_Nacimiento")]
        public DateTime? Fecha_Nacimiento { get; set; }


        [StringLength(9, ErrorMessage = "Máximo 9 caracteres")]
        [RegularExpression(@"^\d{7,9}$", ErrorMessage = "El teléfono debe contener solo números")]
        [Column("Telefono")]
        public string? Telefono { get; set; }
    }
}
