using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ProyectoSantaMonica_Cesar.Models.enums;

namespace ProyectoSantaMonica_Cesar.Models
{
    public class ComprobanteDePago
    {

        [Key]
        [Column("Id_Comprobante")]
        public int Id_Comprobante { get; set; }

       
        [Required(ErrorMessage = "La cita es obligatoria")]
        [Column("Id_Cita")]
        public int Id_Cita { get; set; }

      
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        [Column("Nombre_Pagador")]
        public string Nombre_Pagador { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(100)]
        [Column("Apellidos_Pagador")]
        public string Apellidos_Pagador { get; set; }

        [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 dígitos")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe contener solo números")]
        [Column("Dni_Pagador")]
        public string? Dni_Pagador { get; set; }

        [StringLength(15)]
        [Column("Contacto_Pagador")]
        public string? Contacto_Pagador { get; set; }

       
        [Column("Fecha_Emision")]
        public DateTime Fecha_Emision { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, 999999.99, ErrorMessage = "Monto inválido")]
        [Column("Monto", TypeName = "decimal(10,2)")]
        public decimal Monto { get; set; }

    
        [Required(ErrorMessage = "El método de pago es obligatorio")]
       
        public string Metodo_Pago { get; set; }

       
        [Required]
    
        public EstadoComprobante Estado { get; set; } = EstadoComprobante.EMITIDO;

        [ForeignKey("Id_Cita")]
        public Cita? Cita { get; set; }
    

}
}
