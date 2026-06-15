using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoSantaMonica_Cesar.Models;

namespace ProyectoSantaMonica_Cesar.Controllers
{
    [Authorize(Roles = "RECEPCIONISTA")]
    public class RecepcionistaController : Controller
    {
        public IActionResult Index()
        {
            var rolString = User.FindFirst(ClaimTypes.Role)?.Value;

            var usuario = new Usuario
            {
                Nombres = User.Identity?.Name,
                Img_Perfil = "/images/default-user.jpg",
                Rol = Enum.Parse<Roles>(rolString) 
            };

            return View(usuario);
        }
    }
}