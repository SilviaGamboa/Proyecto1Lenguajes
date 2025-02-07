using Microsoft.AspNetCore.Mvc;
using ProyectoI.Models;
using ProyectoI.Repositories;
using System.Diagnostics;
using System.Text.Json;

namespace ProyectoI.Controllers
{
    public class TareaController : Controller
    {
        private readonly ILogger<TareaController> _logger;

        private readonly HttpClient _httpClient;
        private readonly ITareasRepository _tareaRepository;
        private readonly IEmailRepository _emailRepository;
        private readonly IUserRepository _userRepository;


        public TareaController(ILogger<TareaController> logger, HttpClient httpClient, ITareasRepository tareaRepository, IEmailRepository emailRepository, IUserRepository userRepository)
        {
            _logger = logger;
            _httpClient = httpClient;
            _tareaRepository = tareaRepository;
            _emailRepository = emailRepository;
            _userRepository = userRepository;
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public IActionResult CrearTarea()
        {
            return View("CrearTarea");
        }

        [HttpPost]
        public IActionResult CrearTarea(TareaModel tarea)
        {
            //el id de la tarea se puede agregar en el frot, borrar esto
            //tarea.UsuarioCreadorId = 1;
            _tareaRepository.CreateTarea(tarea);
            return RedirectToAction("GetTareas");
        }

        [HttpGet]
        public IActionResult GetTareas()
        {
            List<TareaModel> tareas = _tareaRepository.GetAllTareas();
            return View("GetTareas", tareas);
        }
        [HttpGet]
        public IActionResult ActualizarTarea(int id)
        {
            var tarea = _tareaRepository.GetTareaById(id);
            if (tarea == null)
            {
                Console.WriteLine(id + "aa");
                return NotFound();
            }

            return View("ActualizarTarea", tarea);
        }

        [HttpPost]
        public IActionResult ActualizarTarea(TareaModel tarea)
        {
            if (tarea == null || tarea.Id <= 0)
            {
                return BadRequest("Datos de tarea inválidos");
            }
            var existeTarea = _tareaRepository.GetTareaById(tarea.Id);
            if (existeTarea == null)
            {
                return NotFound("Tarea no encontrada");
            }

            _tareaRepository.UpdateTarea(tarea);
            return RedirectToAction("GetTareas");
        }

        [HttpPost]
        public IActionResult EliminarTarea(int id)
        {
            var tarea = _tareaRepository.GetTareaById(id);
            if (tarea == null)
            {
                return NotFound("Tarea no encontrada");
            }

            _tareaRepository.DeleteTareaByID(id);
            return RedirectToAction("GetTareas");
        }

        [HttpPost]
        public async Task<IActionResult> AsignarUsuario(int tareaId, int usuarioId)
        {
            bool asignado = _tareaRepository.AsignarUsuarioATarea(tareaId, usuarioId);
            if (!asignado)
            {
                return BadRequest("No se pudo asignar el usuario a la tarea.");
            }

            // Obtener los datos del usuario y tarea asignados
            var tarea = _tareaRepository.GetTareaById(tareaId);
            var usuario = _userRepository.GetUserByID(usuarioId);

            // Enviar correo notificando la asignación de tarea
            bool correoEnviado = await _emailRepository.EnviarCorreoAsignacionTareaAsync(usuario.Correo, usuario.Nombre, tarea.Titulo);
            if (!correoEnviado)
            {
                _logger.LogError("Error al enviar correo de asignación de tarea.");
            }

            return RedirectToAction("GetTareas");
        }

        [HttpGet]
        public IActionResult ObtenerTarea(int id)
        {
            // Obtener la tarea por su ID
            var tarea = _tareaRepository.GetTareaById(id);

            if (tarea == null)
            {
                // Si la tarea no existe, devuelve un NotFound (404)
                return NotFound();
            }

            // Si la tarea existe, devuelve un objeto JSON
            return Json(tarea);
        }
        [HttpPost]
        public async Task<IActionResult> CambiarEstado([FromBody] JsonElement request)
        {
            if (request.ValueKind == JsonValueKind.Undefined ||
                !request.TryGetProperty("tareaId", out var tareaId) ||
                !request.TryGetProperty("nuevoEstado", out var nuevoEstado))
            {
                return BadRequest(new { message = "Datos inválidos." });  // Devolver como JSON
            }

            // Verificamos que los valores no sean nulos o vacíos
            if (tareaId.ValueKind != JsonValueKind.Number || nuevoEstado.ValueKind != JsonValueKind.String)
            {
                return BadRequest(new { message = "Datos inválidos." + request });  // Devolver como JSON
            }

            // Obtenemos los valores de tareaId y nuevoEstado
            int tareaIdValue = tareaId.GetInt32();
            string nuevoEstadoValue = nuevoEstado.GetString();

            // Llamamos al repositorio para cambiar el estado
            bool cambiado = _tareaRepository.CambiarEstadoTarea(tareaIdValue, nuevoEstadoValue);

            if (!cambiado)
            {
                return BadRequest(new { message = "No se pudo cambiar el estado." });  // Devolver como JSON
            }

            // Obtener los datos de la tarea y el usuario asignado
            var tarea = _tareaRepository.GetTareaById(tareaIdValue);

            if (tarea.UsuarioAsignadoId.HasValue)
            {
                var usuario = _userRepository.GetUserByID(tarea.UsuarioAsignadoId.Value);
                // Enviar correo notificando el cambio de estado
                bool correoEnviado = await _emailRepository.EnviarCorreoCambioEstadoTareaAsync(usuario.Correo, usuario.Nombre, tarea.Titulo, nuevoEstadoValue);
                if (!correoEnviado)
                {
                    _logger.LogError("Error al enviar correo de cambio de estado de tarea.");
                }
            } 

            return Ok(new { message = "Estado actualizado correctamente.", tareaId = tareaIdValue, nuevoEstado = nuevoEstadoValue });
        }
    }
}
