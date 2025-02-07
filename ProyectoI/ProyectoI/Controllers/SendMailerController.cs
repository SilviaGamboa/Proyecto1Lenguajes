using Microsoft.AspNetCore.Mvc;
using ProyectoI.Models;
using ProyectoI.Repositories;
using System.Threading.Tasks;

namespace ProyectoI.Controllers
{
    public class SendMailerController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailRepository _emailRepository;

        public SendMailerController(IUserRepository userRepository, IEmailRepository emailRepository)
        {
            _userRepository = userRepository;
            _emailRepository = emailRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(MailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string contrasennaGenerada = _emailRepository.GenerarContrasennaSegura();
                    bool resultado = await _userRepository.CreateUserAsync("Invitado", model.To, contrasennaGenerada);

                    if (resultado)
                    {
                        bool emailEnviado = await _emailRepository.SendEmailAsync(model, contrasennaGenerada);
                        if (emailEnviado)
                        {
                            ModelState.Clear();
                            return View(new MailModel());
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Error al enviar el correo.";
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Error al crear usuario.";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"Error: {ex.Message}";
                }
            }
            return View();
        }
    }
}

