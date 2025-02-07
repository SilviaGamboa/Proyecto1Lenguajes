using Microsoft.AspNetCore.Mvc;
using ProyectoI.Repositories;
using System.Threading.Tasks;

namespace ProyectoI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncriptadorRepository _encriptadorRepository; // Inyectar el repositorio de encriptación

        // Constructor con inyección de dependencias
        public AccountController(IUserRepository userRepository, IEncriptadorRepository encriptadorRepository)
        {
            _userRepository = userRepository;
            _encriptadorRepository = encriptadorRepository;  // Guardar la referencia del repositorio de encriptación
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string contrasenna)
        {
            // Autenticar al usuario utilizando el correo
            var user = await _userRepository.AuthenticateUserAsync(correo);

            if (user != null)
            {
                // Verificar si la contraseña ingresada coincide con el hash almacenado
                bool isPasswordCorrect = _encriptadorRepository.Verificar(user.Contrasenna, contrasenna);

                if (isPasswordCorrect)
                {
                    // Determinar si el usuario es admin
                    bool isAdmin = correo == "admin@example.com"; // Si el correo es "admin", isAdmin será true

                    // Si la contraseña es correcta, retornar un JSON con la información del usuario
                    return Json(new
                    {
                        success = true,
                        isAdmin = isAdmin,
                        user = new
                        {
                            id = user.Id,
                            nombre = user.Nombre,
                            correo = user.Correo
                        }
                    });
                }
            }

            // Si no es correcto o el usuario no existe, retornar mensaje de error
            return Json(new { success = false, message = "Correo o contraseña incorrectos." });
        }

        [HttpGet]
        public IActionResult CerrarSesion()
        {
            return View(); 
        }


    }
}

