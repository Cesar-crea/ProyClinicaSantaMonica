using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Repository;
using ProyectoSantaMonica_Cesar.Service.MedicoService;

namespace ProyectoSantaMonica_Cesar.Controllers
{
    public class MedicoController : Controller
    {
       
        private readonly EspecialidadRepository _especialidadRepository;
        private readonly IMedicoService _medicoService;

        public MedicoController(IMedicoService medicoService, EspecialidadRepository especialidadRepository)
        {
            _medicoService = medicoService;
            _especialidadRepository = especialidadRepository;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _medicoService.ListarTodosAsync();
            return View(lista);
        }

        public async Task< IActionResult> Create()
        {
            var especialidades = await _especialidadRepository.Listar();

            ViewBag.Especialidades = new SelectList(
                especialidades,
                nameof(Especialidad.Id_Especialidad),
                nameof(Especialidad.Nombre)
            );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medico medico)
        {
            if (!ModelState.IsValid)
            {
                var especialidades = await _especialidadRepository.Listar();
                ViewBag.Especialidades = new SelectList(
                    especialidades, 
                    nameof(Especialidad.Id_Especialidad),
                    nameof(Especialidad.Nombre)
                    );
                return View(medico);
            }

            await _medicoService.GuardarAsync(medico);
            TempData["mensaje"] = "Medico registrado correctamente";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var medico = await _medicoService.BuscarPorIdAsync(id);
            if (medico == null) return NotFound();

            var especialidades = await _especialidadRepository.Listar();

            ViewBag.Especialidades = new SelectList(
                especialidades,
               nameof(Especialidad.Id_Especialidad),
               nameof(Especialidad.Nombre)
                ,
                medico.Id_Especialidad // seleccionado
            );

            return View(medico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Medico medico)
        {
            if (!ModelState.IsValid)
            {
                var especialidades = await _especialidadRepository.Listar();

                ViewBag.Especialidades = new SelectList(
                    especialidades,
                    nameof(Especialidad.Id_Especialidad),
                    nameof(Especialidad.Nombre),
                    medico.Id_Especialidad
                );
                return View(medico);
            }

                medico.Id_Medico = id;
            await _medicoService.ActualizarAsync(medico);

            TempData["mensaje"] = "Medico actualizado correctamente";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _medicoService.EliminarAsync(id);
            TempData["mensaje"] = "Medico eliminado correctamente";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<JsonResult> Buscar(string texto = "")
        {
            List<Medico> lista;

            if (string.IsNullOrWhiteSpace(texto))
            {
                lista = await _medicoService.ListarTodosAsync();
            }
            else
            {
                lista = await _medicoService.BuscarGeneralAsync(texto);
            }

            var data = lista.Select(m => new
            {
                id = m.Id_Medico,
                nombres = m.Nombres,
                apellidos = m.Apellidos,
                especialidad = m.Especialidad
            });

            return Json(data);
        }
    }
}