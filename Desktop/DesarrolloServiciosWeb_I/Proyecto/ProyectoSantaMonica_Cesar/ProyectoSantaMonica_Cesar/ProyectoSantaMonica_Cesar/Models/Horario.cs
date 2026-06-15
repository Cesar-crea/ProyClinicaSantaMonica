using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ProyectoSantaMonica_Cesar.Models.enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProyectoSantaMonica_Cesar.Models
{
    public class Horario
    {
     
        [Key]
        [Column("Id_Horario")]
        public int Id_Horario { get; set; }

     
        [Required(ErrorMessage = "Debe seleccionar un médico")]
        [Column("Id_Medico")]
        public int Id_Medico { get; set; }

       
        [Required(ErrorMessage = "Debe seleccionar un día")]
        [Column("Dia_Semana")]
        public DiaSemana ? Dia_Semana { get; set; }

      


        
        [DataType(DataType.Time)]
        [Required(ErrorMessage = "Debe ingresar hora de entrada")]
        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? Horario_Entrada { get; set; }

        [DataType(DataType.Time)]
        [Required(ErrorMessage = "Debe ingresar hora de salida")]
        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? Horario_Salida { get; set; }

        [ValidateNever]
        public Medico? Medico { get; set; }
    }



}
