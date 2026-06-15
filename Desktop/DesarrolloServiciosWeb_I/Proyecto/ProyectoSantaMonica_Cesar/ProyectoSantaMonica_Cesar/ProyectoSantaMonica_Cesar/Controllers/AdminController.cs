using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Service.UsuarioService;

namespace ProyectoSantaMonica_Cesar.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR")]
    public class AdminController : Controller
    {
        private readonly IUsuarioService service;
        private readonly IWebHostEnvironment env;

        public AdminController(IUsuarioService service, IWebHostEnvironment env)
        {
            this.service = service;
            this.env = env;
        }
        public IActionResult Index()
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Usuario");

            var usuario = service.BuscarPorUserName(username);

            return View(usuario); // Views/Admin/Index.cshtml
        }

        public IActionResult Registro()
        {
            return View(new Usuario());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registro(Usuario usuario, IFormFile archivoImagen)
        {
            try
            {
                string carpeta = Path.Combine(env.WebRootPath, "uploads");

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
                    usuario.Img_Perfil = "/images/default-user.jpg";
                }

                var resultado = service.RegistrarUsuario(usuario);

                if (resultado != "OK")
                {
                    TempData["error"] = resultado;
                    return View(usuario);
                }

                TempData["mensaje"] = "Usuario registrado correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(usuario);
            }
        }


        // ==========================
        // 🔍 EDITAR (GET - BUSCAR)
        // ==========================
        public IActionResult Editar(string nombre)
        {
            if (!string.IsNullOrEmpty(nombre))
            {
                try
                {
                    var usuario = service.BuscarPorNombre(nombre);
                    return View(usuario);
                }
                catch (Exception ex)
                {
                    TempData["error"] = ex.Message;
                }
            }

            return View();
        }

        // ==========================
        // ✏️ ACTUALIZAR (POST)
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Actualizar(Usuario usuario, IFormFile archivoImagen)
        {
            try
            {
                var antiguo = service.BuscarPorID(usuario.Id_Usuario);

                string carpeta = Path.Combine(env.WebRootPath, "uploads");

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

                TempData["mensaje"] = "Usuario actualizado correctamente";

                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Editar", new { nombre = usuario.Nombres });
            }
        }

    }

    
}