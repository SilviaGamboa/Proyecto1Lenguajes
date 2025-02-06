using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoI.Repositories;

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
                // Aquí puedes agregar la lógica para almacenar al usuario en la sesión o en un token de autenticación.
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Correo o contraseña incorrectos.";
            return View();
        }
    }
}
