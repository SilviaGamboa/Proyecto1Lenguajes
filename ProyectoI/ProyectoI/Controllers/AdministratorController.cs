using Microsoft.AspNetCore.Mvc;
using ProyectoI.Repositories;
using ProyectoI.Models;
using System.Threading.Tasks;

namespace ProyectoI.Controllers
{
    public class AdministratorController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncriptadorRepository _encriptadorRepository;  // Inyectar el repositorio de encriptación

        public AdministratorController(IUserRepository userRepository, IEncriptadorRepository encriptadorRepository)
        {
            _userRepository = userRepository;
            _encriptadorRepository = encriptadorRepository;  // Guardar la referencia del repositorio de encriptación
        }

        // Método para mostrar la vista de gestión de usuarios
        public IActionResult GestionUsuario()
        {
            return View();
        }

        // Crear usuario manualmente
        [HttpPost]
        public async Task<IActionResult> GestionUsuario(string nombre, string correo, string contrasenna)
        {
            // Encriptar la contraseña antes de guardarla
            var contrasennaEncriptada = _encriptadorRepository.Encriptar(contrasenna);

            // Crear usuario con la contraseña encriptada
            bool resultado = await _userRepository.CreateUserAsync(nombre, correo, contrasennaEncriptada);

            if (resultado)
            {
                ViewBag.Mensaje = "Usuario creado exitosamente.";
            }
            else
            {
                ViewBag.ErrorMessage = "Error al crear usuario.";
            }

            return RedirectToAction("GestionUsuario");
        }

        // Obtener usuarios
        [HttpGet]
        public IActionResult GetUsers()
        {
            List<UserModel> users = _userRepository.GetAllUsers();
            return Json(users);
        }

        // Obtener usuario por id
        [HttpGet]
        public IActionResult GetUserById(int id)
        {
            var user = _userRepository.GetUserByID(id);
            if (user == null)
            {
                return NotFound("Usuario no encontrado");
            }
            return Json(user);
        }

        // Mostrar la vista de Perfil de Usuario
        public IActionResult Perfil()
        {
            return View();
        }

        // Actualizar perfil de usuario
        [HttpPost]
        public IActionResult UpdateProfile([FromBody] UserModel user)
        {
            var existingUser = _userRepository.GetUserByID(user.Id);

            if (existingUser == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            existingUser.Nombre = user.Nombre;
            existingUser.Correo = user.Correo;

            _userRepository.UpdateUser(existingUser);

            return Json(new { success = true, message = "Perfil actualizado correctamente" });
        }
    }
}
