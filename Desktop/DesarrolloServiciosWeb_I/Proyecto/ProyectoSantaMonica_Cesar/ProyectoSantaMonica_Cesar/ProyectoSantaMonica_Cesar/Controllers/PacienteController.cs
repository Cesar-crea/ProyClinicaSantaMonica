using Microsoft.AspNetCore.Mvc;
using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Repository;
using ProyectoSantaMonica_Cesar.Service.PacienteService;

namespace ProyectoSantaMonica_Cesar.Controllers
{
    public class PacienteController : Controller
    {
        private readonly IPacienteService _service;

        public PacienteController(IPacienteService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            var lista = _service.ListarTodosPaciente();
            return View(lista);
        }

        public IActionResult Create()
        {
            return View(new Paciente());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Paciente paciente)
        {
            if (!ModelState.IsValid)
                return View(paciente);

            _service.GuardarPaciente(paciente);
            TempData["mensaje"] = "Paciente registrado correctamente";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var paciente = _service.BuscarPacientePorId(id);
            return View(paciente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Paciente paciente)
        {
            _service.ActualizarPaciente(paciente);
            TempData["mensaje"] = "Paciente actualizado";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            _service.EliminarPaciente(id);
            TempData["mensaje"] = "Paciente eliminado";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult Buscar(string texto = "")
        {
            List<Paciente> lista;

            if (string.IsNullOrWhiteSpace(texto))
            {
                lista = _service.ListarTodosPaciente();
            }
            else
            {
                // ✅ AQUÍ ESTÁ EL CAMBIO
                lista = _service.BuscarGeneral(texto);
            }

            var data = lista.Select(p => new
            {
                id = p.Id_Paciente,
                nombres = p.Nombres,
                apellidos = p.Apellidos,
                dni = p.Dni
            });

            return Json(data);
        }

    }
}
