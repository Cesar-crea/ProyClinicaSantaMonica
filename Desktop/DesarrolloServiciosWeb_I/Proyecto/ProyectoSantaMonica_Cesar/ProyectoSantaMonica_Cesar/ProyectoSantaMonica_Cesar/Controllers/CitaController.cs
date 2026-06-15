using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Models.enums;
using ProyectoSantaMonica_Cesar.Models.ViewModels;
using ProyectoSantaMonica_Cesar.Service.CitaService;
using ProyectoSantaMonica_Cesar.Service.MedicoService;
using ProyectoSantaMonica_Cesar.Service.PacienteService;
using ProyectoSantaMonica_Cesar.Data; // o donde esté tu DbContext

namespace ProyectoSantaMonica_Cesar.Controllers
{
    public class CitaController : Controller
    {
        private readonly ICitaService _service;
        private readonly IPacienteService _pacienteservice;
        private readonly IMedicoService _medicoservice;
        private readonly ApplicationDbContext _context;
        public CitaController(ICitaService service, IPacienteService pacienteService,
           IMedicoService medicoService, ApplicationDbContext context)
        {
            _service = service;
            _pacienteservice = pacienteService;
            _medicoservice = medicoService;
            _context = context;
        }

  

//  LISTAR (con búsqueda)
public async Task<IActionResult> Listar(string? texto)
        {
            try
            {
                var lista = string.IsNullOrEmpty(texto)
                    ? await _service.ListarAsync()
                    : await _service.BuscarPorPacienteAsync(texto);

                ViewBag.Texto = texto;
                return View(lista);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<Cita>());
            }
        }

        public async Task<IActionResult> Registrar()
        {
            ViewBag.Pacientes = _pacienteservice.ListarTodosPaciente();
            ViewBag.Medicos = await _medicoservice.ListarTodosAsync();

            return View(new Cita());
        }

        // REGISTRAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(Cita c)
        {
            try
            {
                // 👇 Usuario logueado
                c.Id_Usuario = Convert.ToInt64(User.FindFirst("Id_Usuario")?.Value);

                // ===================================================
                // VALIDAR SI EL MEDICO ATIENDE EN ESE HORARIO
                // ===================================================

                bool atiende = await _service
                    .MedicoAtiendeEnHorarioAsync(
                        c.Id_Medico,
                        c.Fecha,
                        c.Hora);

                if (!atiende)
                {
                    TempData["Error"] = "El médico no atiende en ese horario";

                    ViewBag.Pacientes = _pacienteservice.ListarTodosPaciente();
                    ViewBag.Medicos = await _medicoservice.ListarTodosAsync();

                    return View(c);
                }

                // ===================================================
                // REGISTRAR
                // ===================================================

                var msg = await _service.RegistrarAsync(c);

                TempData[msg == "OK" ? "Success" : "Error"] =
                    msg == "OK"
                    ? "Cita registrada correctamente"
                    : msg;

                return RedirectToAction(nameof(Listar));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                ViewBag.Pacientes = _pacienteservice.ListarTodosPaciente();
                ViewBag.Medicos = await _medicoservice.ListarTodosAsync();

                return View(c);
            }
        }

        //  ACTUALIZAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Actualizar(Cita c)
        {
            try
            {
                var msg = await _service.ActualizarAsync(c);

                TempData[msg == "OK" ? "Success" : "Error"] =
                    msg == "OK" ? "Cita actualizada correctamente" : msg;
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Listar));
        }

        //  ELIMINAR
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var msg = await _service.EliminarAsync(id);

                TempData[msg == "OK" ? "Success" : "Error"] =
                    msg == "OK" ? "Cita eliminada correctamente" : msg;
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Listar));
        }

        //  DETALLE
        public async Task<IActionResult> Detalle(int id)
        {
            try
            {
                var cita = await _service.ObtenerPorIdAsync(id);

                if (cita == null)
                {
                    TempData["Error"] = "Cita no encontrada";
                    return RedirectToAction(nameof(Listar));
                }

                return View(cita);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }

        //  CANCELAR
        public async Task<IActionResult> Cancelar(int id)
        {
            try
            {
                var cita = await _service.ObtenerPorIdAsync(id);

                if (cita == null)
                {
                    TempData["Error"] = "Cita no encontrada";
                    return RedirectToAction(nameof(Listar));
                }

                if (cita.Estado != EstadoCita.PENDIENTE)
                {
                    TempData["Error"] = "Solo se pueden cancelar citas pendientes";
                    return RedirectToAction(nameof(Listar));
                }

                var msg = await _service.ActualizarEstadoAsync(id, EstadoCita.CANCELADO);

                TempData[msg == "OK" ? "Success" : "Error"] =
                    msg == "OK" ? "Cita cancelada correctamente" : msg;
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Listar));
        }

        //  FILTRAR POR ESTADO
        public async Task<IActionResult> PorEstado(EstadoCita estado)
        {
            var lista = await _service.ListarPorEstadoAsync(estado);
            return View("Listar", lista);
        }

        //  PENDIENTES POR PACIENTE
        public async Task<IActionResult> Pendientes(string dato)
        {
            var lista = await _service.PendientesPorPacienteAsync(dato);
            return View("Listar", lista);
        }

        public async Task<IActionResult> ActualizarEstados()
        {
            try
            {
                await _service.ActualizarEstadosAutomaticosAsync();
                TempData["Success"] = "Estados actualizados correctamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Listar));
        }

        [HttpGet]
        public async Task<IActionResult> HorariosOcupados(int idMedico, DateTime fecha)
        {
            var horarios = await _medicoservice
                .ObtenerHorariosPorMedicoAsync(idMedico)
                ?? new List<Horario>();

            var ocupadas = await _context.Cita
                .Include(c => c.Paciente)
                .Where(c =>
                    c.Id_Medico == idMedico &&
                    c.Fecha.Date == fecha.Date)
                .OrderBy(c => c.Hora)
                .ToListAsync();

            var vm = new HorariosViewModel
            {
                Horarios = horarios,
                Ocupadas = ocupadas
            };

            return PartialView("_ModalHorarios", vm);
        }


        //Buscar Cita

        [HttpGet]
        public async Task<JsonResult> Buscar(string texto)
        {
            List<Cita> lista;

            if (string.IsNullOrEmpty(texto))
            {
                lista = await _service.ListarPorEstadoAsync(EstadoCita.PENDIENTE);
            }
            else
            {
                lista = await _service.PendientesPorPacienteAsync(texto);
            }

            var resultado = lista.Select(c => new
            {
                idCita = c.Id_Cita,
                fecha = c.Fecha.ToString("dd/MM/yyyy"),
                hora = c.Hora,
                motivo = c.Motivo,

                pacienteNombre = c.Paciente != null ? c.Paciente.Nombres : "",
                pacienteApellido = c.Paciente != null ? c.Paciente.Apellidos : "",

                medicoNombre = c.Medico != null ? c.Medico.Nombres : "",
                medicoApellido = c.Medico != null ? c.Medico.Apellidos : ""
            });

            return Json(resultado); 
        }

        [HttpGet]
        public async Task<IActionResult> Horarios(int idMedico)
        {
            var horarios = await _medicoservice.ObtenerHorariosPorMedicoAsync(idMedico);
            var ocupadas = await _service.HorariosOcupadosAsync(idMedico, DateTime.Today);

            var vm = new HorariosViewModel
            {
                Horarios = horarios,
                Ocupadas = ocupadas
            };

            return PartialView("_ModalHorarios", vm);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var cita = await _service.ObtenerPorIdAsync(id);

            if (cita == null)
                return NotFound();

            // 🔥 CARGAR DATOS PARA MODALES
            ViewBag.Pacientes = _pacienteservice.ListarTodosPaciente();
            ViewBag.Medicos = await _medicoservice.ListarTodosAsync();

            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Cita c)
        {
            try
            {
                // =========================================
                // VALIDAR HORARIO DEL MEDICO
                // =========================================

                bool atiende = await _service
                    .MedicoAtiendeEnHorarioAsync(
                        c.Id_Medico,
                        c.Fecha,
                        c.Hora);

                if (!atiende)
                {
                    TempData["Error"] =
                        "El médico no atiende en ese horario";

                    // =========================================
                    // RECARGAR DATOS COMPLETOS
                    // =========================================

                    var citaOriginal =
                        await _service.ObtenerPorIdAsync(c.Id_Cita);

                    if (citaOriginal != null)
                    {
                        // SOLO ACTUALIZA FECHA Y HORA
                        citaOriginal.Fecha = c.Fecha;
                        citaOriginal.Hora = c.Hora;
                        citaOriginal.Motivo = c.Motivo;
                    }

                    ViewBag.Pacientes =
                        _pacienteservice.ListarTodosPaciente();

                    ViewBag.Medicos =
                        await _medicoservice.ListarTodosAsync();

                    return View(citaOriginal);
                }

                // =========================================
                // ACTUALIZAR
                // =========================================

                var msg = await _service.ActualizarAsync(c);

                TempData[msg == "OK" ? "Success" : "Error"] =
                    msg == "OK"
                    ? "Cita actualizada correctamente"
                    : msg;

                return RedirectToAction(nameof(Listar));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                var citaOriginal =
                    await _service.ObtenerPorIdAsync(c.Id_Cita);

                if (citaOriginal != null)
                {
                    citaOriginal.Fecha = c.Fecha;
                    citaOriginal.Hora = c.Hora;
                    citaOriginal.Motivo = c.Motivo;
                }

                ViewBag.Pacientes =
                    _pacienteservice.ListarTodosPaciente();

                ViewBag.Medicos =
                    await _medicoservice.ListarTodosAsync();

                return View(citaOriginal);
            }
        }

    }
}