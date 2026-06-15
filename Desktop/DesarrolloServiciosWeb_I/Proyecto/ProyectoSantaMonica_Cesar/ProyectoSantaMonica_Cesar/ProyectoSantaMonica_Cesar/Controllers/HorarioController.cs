using Microsoft.AspNetCore.Mvc;
using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Service.HorarioService;
using ProyectoSantaMonica_Cesar.Service.MedicoService;

namespace ProyectoSantaMonica_Cesar.Controllers
{
    public class HorarioController : Controller
    {
        private readonly IHorarioService _horarioService;
        private readonly IMedicoService _medicoService;

        public HorarioController(IHorarioService horarioService, IMedicoService medicoService)
        {
            _horarioService = horarioService;
            _medicoService = medicoService;
        }

        // 🔹 GET: Gestionar Horarios
        public async Task<IActionResult> GestionarHorarios(int? idMedico)
        {
            var listaMedicos = await _medicoService.ListarTodosAsync();
            ViewBag.ListaMedicos = listaMedicos;

            Medico? medico = null;
            var listaHorarios = new List<Horario>();

            if (idMedico.HasValue && idMedico.Value > 0)
            {
                medico = await _medicoService.BuscarPorIdAsync(idMedico.Value);
                listaHorarios = await _horarioService.ListarPorMedicoAsync(idMedico.Value);
            }

            ViewBag.Medico = medico;
            ViewBag.ListaHorarios = listaHorarios;
            ViewBag.IdMedico = idMedico;

            return View("~/Views/Horario/GestionarHorarios.cshtml", new Horario
            {
                Id_Medico = idMedico ?? 0
            });
        }

        // 🔹 AJAX
        [HttpGet]
        public async Task<IActionResult> BuscarMedicoJson(string? nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return Json(await _medicoService.ListarTodosAsync());

            return Json(await _medicoService.BuscarPorNombreAsync(nombre));
        }

        // 🔹 REGISTRAR / ACTUALIZAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(Horario horario)
        {
            try
            {
                // 🔥 VALIDACIÓN REAL (CORREGIDO)
                if (!ModelState.IsValid)
                {
                    // Obtener errores reales
                    var errores = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    TempData["Error"] = string.Join(" | ", errores);

                    // 🔥 RECARGAR DATOS (IMPORTANTE)
                    var listaMedicos = await _medicoService.ListarTodosAsync();
                    ViewBag.ListaMedicos = listaMedicos;

                    var medico = await _medicoService.BuscarPorIdAsync(horario.Id_Medico);
                    var listaHorarios = await _horarioService.ListarPorMedicoAsync(horario.Id_Medico);

                    ViewBag.Medico = medico;
                    ViewBag.ListaHorarios = listaHorarios;
                    ViewBag.IdMedico = horario.Id_Medico;

                    // 🔥 NO REDIRECT → DEVOLVER VISTA
                    return View("~/Views/Horario/GestionarHorarios.cshtml", horario);
                }

                if (horario.Id_Medico == 0)
                {
                    TempData["Error"] = "Debe seleccionar un médico";
                    return RedirectToAction(nameof(GestionarHorarios));
                }

                if (horario.Id_Horario == 0)
                {
                    await _horarioService.RegistrarAsync(horario);
                    TempData["Success"] = "Horario registrado correctamente";
                }
                else
                {
                    await _horarioService.ActualizarAsync(horario);
                    TempData["Success"] = "Horario actualizado correctamente";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(GestionarHorarios), new { idMedico = horario.Id_Medico });
        }

        // 🔹 ELIMINAR
        [HttpGet]
        public async Task<IActionResult> Eliminar(int id, int idMedico)
        {
            try
            {
                await _horarioService.EliminarAsync(id);
                TempData["Success"] = "Horario eliminado correctamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(GestionarHorarios), new { idMedico });
        }
    }
}