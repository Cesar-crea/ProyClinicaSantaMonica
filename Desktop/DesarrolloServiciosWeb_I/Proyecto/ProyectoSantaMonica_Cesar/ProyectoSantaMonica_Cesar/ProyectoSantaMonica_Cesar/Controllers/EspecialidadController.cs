using Microsoft.AspNetCore.Mvc;
using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Service.EspecialidadService;

namespace ProyectoSantaMonica_Cesar.Controllers
{
    public class EspecialidadController : Controller
    {
        private readonly IEspecialidadService _service;

        public EspecialidadController(IEspecialidadService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Listar()
        {
            var lista = await _service.Listar();
            return View(lista);
        }

        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(Especialidad e)
        {
            var result = await _service.Registrar(e);

            if (result == "OK")
            {
                TempData["success"] = "Especialidad registrada correctamente";
                return RedirectToAction("Listar");
            }
            ModelState.AddModelError("", result);

            return View(e);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var esp = await _service.ObtenerPorId(id);
            return View(esp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Especialidad e)
        {
            var result = await _service.Actualizar(e);

            if (result == "OK")
            {
                TempData["success"] = "Especialidad actualizada correctamente"; 
                return RedirectToAction("Listar");
            }

            ModelState.AddModelError("", result);
            return View(e);
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            await _service.Eliminar(id);
            return RedirectToAction("Listar");
        }

        [HttpGet]
        public async Task<IActionResult> ExisteNombre(string nombre, int id = 0)
        {
            var lista = await _service.Listar();

            var existe = lista.Any(x =>
                x.Nombre.ToLower() == nombre.ToLower()
                && x.Id_Especialidad != id // importante para editar
            );

            return Json(new { existe });
        }

    }
}