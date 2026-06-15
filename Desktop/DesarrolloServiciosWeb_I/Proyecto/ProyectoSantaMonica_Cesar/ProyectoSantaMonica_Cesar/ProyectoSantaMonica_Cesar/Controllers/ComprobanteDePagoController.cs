using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSantaMonica_Cesar.Models;
using ProyectoSantaMonica_Cesar.Service.ComprobanteDePagoService;
using ProyectoSantaMonica_Cesar.Data; // o donde esté tu DbContext

namespace ProyectoSantaMonica_Cesar.Controllers
{
    public class ComprobanteDePagoController : Controller
    {
        private readonly IComprobanteDePagoService service;
        private readonly ApplicationDbContext _context;

        public ComprobanteDePagoController(IComprobanteDePagoService service, ApplicationDbContext context)
        {
            this.service = service;
            _context = context;
        }

        public async Task<IActionResult> Listar(string texto)
        {
            var lista = string.IsNullOrEmpty(texto)
                ? await service.ListarAsync()
                : await service.BuscarAsync(texto);

            ViewBag.Texto = texto;
            return View(lista);
        }

        public IActionResult Registrar()
        {
            return View(new ComprobanteDePago());
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(ComprobanteDePago c)
        {
            try
            {
                // 🔴 VALIDAR DUPLICADO
                var existe = _context.ComprobanteDePago
                    .Any(cp => cp.Id_Cita == c.Id_Cita);

                if (existe)
                {
                    TempData["error"] = "La cita ya tiene un comprobante registrado.";
                    return RedirectToAction("Registrar");
                }

                // 🔥 CAMBIAR ESTADO DE LA CITA
                var cita = _context.Cita.Find(c.Id_Cita);

                if (cita != null)
                {
                    cita.Estado = Models.enums.EstadoCita.PAGADO;
                }

                // 💾 GUARDAR
                await service.GuardarAsync(c);

                TempData["success"] = "Comprobante registrado correctamente";
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error al registrar: " + ex.Message;
            }

            return RedirectToAction("Listar");
        }

        public async Task<IActionResult> Anular(int id)
        {
            await service.AnularAsync(id);
            return RedirectToAction("Listar");
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var c = await service.BuscarPorIdAsync(id);
            return View(c);
        }

        [HttpGet]
        public async Task<IActionResult> Buscar(string texto)
        {
            var lista = await service.BuscarPorPacienteAsync(texto);
            return Json(lista);


        }

        [HttpGet]
        public IActionResult BuscarCitas(string texto)
        {
            var citas = _context.Cita
                .Include(c => c.Paciente)
                .Include(c => c.Medico)
                .Where(c =>
                    c.Estado == Models.enums.EstadoCita.PENDIENTE // 🔥 SOLO PENDIENTES
                    &&
                    !_context.ComprobanteDePago
                        .Any(cp => cp.Id_Cita == c.Id_Cita) // 🔥 EVITA DUPLICADOS
                    &&
                    (string.IsNullOrEmpty(texto) ||
                     c.Paciente.Nombres.Contains(texto) ||
                     c.Paciente.Apellidos.Contains(texto))
                )
                .Select(c => new
                {
                    id = c.Id_Cita,
                    paciente = c.Paciente.Nombres + " " + c.Paciente.Apellidos,
                    fecha = c.Fecha.ToString("dd/MM/yyyy"),
                    hora = c.Hora,
                    medico = c.Medico.Nombres + " " + c.Medico.Apellidos,
                    motivo = c.Motivo
                })
                .ToList();

            return Json(citas);
        }
    }
}
