using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProyectoSantaMonica_Cesar.Models
{
    public class Usuario
    {
     
            [Key]
            [Column("Id_Usuario")]
            public long Id_Usuario { get; set; }

         
            [Required(ErrorMessage = "El username es obligatorio")]
            [StringLength(50, MinimumLength = 4, ErrorMessage = "Debe tener entre 4 y 50 caracteres")]
            [Column("Username")]
            public string Username { get; set; }

       
            [Required(ErrorMessage = "La contraseña es obligatoria")]
            [StringLength(200, MinimumLength = 6, ErrorMessage = "Mínimo 6 caracteres")]
            [DataType(DataType.Password)]
            [Column("Contrasenia")]
            public string Contrasenia { get; set; }

        
            [Required(ErrorMessage = "Los nombres son obligatorios")]
            [StringLength(100)]
            [Column("Nombres")]
            public string Nombres { get; set; }

  
            [Required(ErrorMessage = "Los apellidos son obligatorios")]
            [StringLength(100)]
            [Column("Apellidos")]
            public string Apellidos { get; set; }

        
            [Required(ErrorMessage = "El DNI es obligatorio")]
            [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 dígitos")]
            [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe contener solo números")]
            [Column("Dni")]
            public string Dni { get; set; }

           
            [StringLength(9, MinimumLength = 9, ErrorMessage = "El teléfono debe tener 9 dígitos")]
            [RegularExpression(@"^\d{9}$", ErrorMessage = "El teléfono debe contener solo números")]
            [Column("Telefono")]
            public string? Telefono { get; set; }

         
            [StringLength(200)]
            [Column("Img_Perfil")]
            public string? Img_Perfil { get; set; }

        
            [EmailAddress(ErrorMessage = "Formato de correo inválido")]
            [StringLength(100)]
            [Column("Correo")]
            public string? Correo { get; set; }

          
            [Required(ErrorMessage = "El rol es obligatorio")]
            [Column("Rol")]
            public Roles Rol { get; set; }
        }
}
