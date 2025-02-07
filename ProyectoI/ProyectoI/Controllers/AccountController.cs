using Microsoft.AspNetCore.Mvc;
using ProyectoI.Repositories;
using System.Threading.Tasks;

namespace ProyectoI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;

        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string contrasenna)
        {
            var user = await _userRepository.AuthenticateUserAsync(correo, contrasenna);

            if (user != null)
            {
                // Retornar un JSON con la información del usuario
                return Json(new
                {
                    success = true,
                    isAdmin = correo == "admin@example.com",
                    user = new
                    {
                        id = user.Id,
                        nombre = user.Nombre,
                        correo = user.Correo
                    }
                });
            }

            return Json(new { success = false, message = "Correo o contraseña incorrectos." });
        }
    }
}
