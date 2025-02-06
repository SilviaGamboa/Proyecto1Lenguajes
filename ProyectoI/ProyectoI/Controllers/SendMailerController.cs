using Microsoft.AspNetCore.Mvc;
using MailKit.Net.Smtp;
using MimeKit;
using ProyectoI.Models;
using ProyectoI.Repositories;
using System.Security.Cryptography;

namespace ProyectoI.Controllers
{
    public class SendMailerController : Controller
    {
        private readonly IUserRepository _userRepository;

        public SendMailerController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Vista para el formulario de envío de correo
        public ActionResult Index()
        {
            return View();
        }

        // Método para enviar correo
        [HttpPost]
        public async Task<ActionResult> Index(MailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string contrasennaGenerada = GenerarContrasennaSegura();

                    bool resultado = await _userRepository.CreateUserAsync("Invitado", model.To, contrasennaGenerada);

                    string cuerpoCorreo = $@"
                <h3>Hola, has sido invitado a registrarte en nuestro sistema.</h3>
                <p>Tu contraseña temporal es: <strong>{contrasennaGenerada}</strong></p>
                <p>Haz clic en el siguiente enlace para completar tu registro:</p>
                <p><a href='https://localhost:7025/' style='background: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Registrarse</a></p>";

                    // Crear un nuevo mensaje de correo
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Aviso de Proyecto de Lenguajes", model.From));
                    message.To.Add(new MailboxAddress("Destinatario", model.To));
                    message.Subject = model.Subject;
                    //message.Body = new TextPart("plain") { Text = model.Body };
                    message.Body = new TextPart("html") { Text = cuerpoCorreo };


                    // Usar MailKit para enviar el correo
                    using (var client = new SmtpClient())
                    {
                        // Conectar al servidor SMTP de Gmail
                        client.Connect("smtp.gmail.com", 587, false);

                        // Autenticación con la contraseña de la aplicación
                        client.Authenticate("kennethkor3206@gmail.com", "hqraykzyqqlpingu");

                        // Enviar el mensaje
                        client.Send(message);
                        client.Disconnect(true);
                    }
                    // 🔹 Después de enviar el correo, pasamos un modelo vacío a la vista
                    ModelState.Clear();
                    return View(new MailModel());
                }
                catch (Exception ex)
                {
                    // Manejar errores
                    ViewBag.ErrorMessage = $"Error al enviar el correo: {ex.Message}";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        private string GenerarContrasennaSegura()
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            char[] password = new char[8];

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] buffer = new byte[8];
                rng.GetBytes(buffer);

                for (int i = 0; i < password.Length; i++)
                {
                    password[i] = caracteres[buffer[i] % caracteres.Length];
                }
            }
            return new string(password);
        }


    }
}
