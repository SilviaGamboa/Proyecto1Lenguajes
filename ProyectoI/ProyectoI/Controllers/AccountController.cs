using Microsoft.AspNetCore.Mvc;
using ProyectoI.Repositories;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            //else if para crear admin en caso de no existir
            else if(correo == "admin@example.com" && contrasenna == "admin")
            {
                
                    // Encriptar la contraseña antes de guardarla
                    var contrasennaEncriptada = _encriptadorRepository.Encriptar(contrasenna);

                    // Crear usuario con la contraseña encriptada
                    bool resultado = await _userRepository.CreateUserAsync("admin", correo, contrasennaEncriptada);

                    if (resultado)
                    {
                        // Autenticar al usuario utilizando el correo
                        var userAdmin = await _userRepository.AuthenticateUserAsync(correo);

                        if (userAdmin != null)
                        {
                            // Verificar si la contraseña ingresada coincide con el hash almacenado
                            bool isPasswordCorrect = _encriptadorRepository.Verificar(userAdmin.Contrasenna, contrasenna);

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
                                        id = userAdmin.Id,
                                        nombre = userAdmin.Nombre,
                                        correo = userAdmin.Correo
                                    }
                                });
                            }
                    }
                    ViewBag.Mensaje = "Usuario creado exitosamente.";
                    }   
                    else
                    {
                        ViewBag.ErrorMessage = "Error al crear usuario.";
                    }

                    

            }//if
            return Json(new { success = false, message = "Correo o contraseña incorrectos." });
        }

        [HttpGet]
        public IActionResult CerrarSesion()
        {
            return View(); 
        }


    }
}

