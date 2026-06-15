using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Service.UsuarioService;
using System.Security.Claims;

namespace ProyectoSantaMonica_Cesar.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService service;
        private readonly IWebHostEnvironment env;

        public UsuarioController(IUsuarioService service, IWebHostEnvironment env)
        {
            this.service = service;
            this.env = env;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        //  LOGIN POST (IMPORTANTE)
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var usuario = service.Login(username, password);

            if (usuario == null)
            {
                ViewBag.error = "Credenciales incorrectas";
                return View();
            }

            //  CLAIMS (CLAVE)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
                 new Claim("Id_Usuario", usuario.Id_Usuario.ToString())
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", principal);

            // REDIRECCIÓN POR ROL
            return usuario.Rol.ToString() switch
            {
                "ADMINISTRADOR" => RedirectToAction("Index", "Admin"),
                "CAJERO" => RedirectToAction("Index", "Cajero"),
                "RECEPCIONISTA" => RedirectToAction("Index", "Recepcionista"),
                _ => RedirectToAction("Login")
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Usuario");
        }

        //  SOLO ADMIN
        [Authorize(Roles = "ADMINISTRADOR")]
        public IActionResult Registro()
        {
            return View("Admin/Registro", new Usuario());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRADOR")]
        public IActionResult Registro(Usuario usuario, IFormFile archivoImagen)
        {
            try
            {
                string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                //  GUARDAR IMAGEN
                if (archivoImagen != null && archivoImagen.Length > 0)
                {
                    string nombre = DateTime.Now.Ticks + "_" + archivoImagen.FileName;
                    string ruta = Path.Combine(carpeta, nombre);

                    using (var stream = new FileStream(ruta, FileMode.Create))
                    {
                        archivoImagen.CopyTo(stream);
                    }

                    usuario.Img_Perfil = "/uploads/" + nombre;
                }
                else
                {
                    usuario.Img_Perfil = "/images/default-user.jpg";
                }

                
                var resultado = service.RegistrarUsuario(usuario);

                if (resultado != "OK")
                {
                    TempData["error"] = resultado;
                    return View("Admin/Registro", usuario);
                }

                TempData["mensaje"] = "Usuario registrado con éxito";
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                // 
                TempData["error"] = ex.Message;
                return View("Admin/Registro", usuario);
            }
        }

        [Authorize(Roles = "ADMINISTRADOR")]
        public IActionResult Editar(string nombre)
        {
            if (!string.IsNullOrEmpty(nombre))
            {
                try
                {
                    var u = service.BuscarPorNombre(nombre);
                    return View("Admin/Editar", u);
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                    return View("Admin/Editar");
                }
            }

            return View("Admin/Editar");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRADOR")]
        public IActionResult Actualizar(Usuario usuario, IFormFile archivoImagen)
        {
            try
            {
                var antiguo = service.BuscarPorID(usuario.Id_Usuario);

                string carpeta = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                if (archivoImagen != null && archivoImagen.Length > 0)
                {
                    string nombre = DateTime.Now.Ticks + "_" + archivoImagen.FileName;
                    string ruta = Path.Combine(carpeta, nombre);

                    using (var stream = new FileStream(ruta, FileMode.Create))
                    {
                        archivoImagen.CopyTo(stream);
                    }

                    usuario.Img_Perfil = "/uploads/" + nombre;
                }
                else
                {
                    usuario.Img_Perfil = antiguo.Img_Perfil;
                }

                service.ActualizarUsuario(usuario, antiguo);

                ViewBag.mensaje = "Usuario actualizado correctamente";
            }
            catch
            {
                ViewBag.error = "Error al actualizar";
            }

            return View("Admin/Editar", usuario);
        }
    }
}