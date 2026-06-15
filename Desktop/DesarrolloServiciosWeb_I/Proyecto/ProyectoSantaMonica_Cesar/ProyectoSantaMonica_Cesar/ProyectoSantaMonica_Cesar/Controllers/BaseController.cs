using Microsoft.AspNetCore.Mvc;
using ProyectoSantaMonica_Cesar.Data;
using ProyectoSantaMonica_Cesar.Models;

namespace ProyectoSantaMonica_Cesar.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;

        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        protected Usuario UsuarioActual
        {
            get
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    string username = User.Identity.Name;

                    return _context.Usuario
                                   .FirstOrDefault(u => u.Username == username);
                }
                return null;
            }
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            ViewBag.UsuarioActual = UsuarioActual;
            base.OnActionExecuting(context);
        }
    }
}
