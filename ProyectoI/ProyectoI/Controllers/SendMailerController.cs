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
        private readonly IEncriptadorRepository _encriptadorRepository; 


        public SendMailerController(IUserRepository userRepository, IEmailRepository emailRepository, IEncriptadorRepository encriptadorRepository)
        {
            _userRepository = userRepository;
            _emailRepository = emailRepository;
            _encriptadorRepository = encriptadorRepository; 

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
                    var usuarioExistente = _userRepository.GetUserByCorreo(model.To);
                    if (usuarioExistente != null)
                    {
                        ViewBag.ErrorMessage = "El correo ya está registrado o ya fue invitado.";
                        return View(model);
                    }

                    string contrasennaGenerada = _emailRepository.GenerarContrasennaSegura();

                    string contrasennaEncriptada = _encriptadorRepository.Encriptar(contrasennaGenerada);

                    bool resultado = await _userRepository.CreateUserAsync("Invitado", model.To, contrasennaEncriptada);

                    if (resultado)
                    {
                        bool emailEnviado = await _emailRepository.SendEmailAsync(model, contrasennaGenerada);
                        if (emailEnviado)
                        {
                            ViewBag.SuccessMessage = "Invitación enviada correctamente";
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

