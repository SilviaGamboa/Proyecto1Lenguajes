using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using ProyectoI.Models;

namespace ProyectoI.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IConfiguration _configuration;

        public EmailRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(MailModel model, string contrasennaGenerada)
        {
            try
            {
                string cuerpoCorreo = $@"
                <h3>Hola, has sido invitado a registrarte en nuestro sistema.</h3>
                <p>Tu contraseña temporal es: <strong>{contrasennaGenerada}</strong></p>
                <p>Haz clic en el siguiente enlace para completar tu registro:</p>
                <p><a href='https://localhost:7025/' style='background: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Registrarse</a></p>";

                return await EnviarCorreoAsync(model.To, "Registro en el sistema", cuerpoCorreo);
            }
            catch
            {
                return false;
            }
        }

        public string GenerarContrasennaSegura()
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            char[] password = new char[8];

            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
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

        // 🔹 Nuevo: Notificar asignación de tarea
        public async Task<bool> EnviarCorreoAsignacionTareaAsync(string correoDestino, string nombreUsuario, string tituloTarea)
        {
            string cuerpoCorreo = $@"
                <h3>Hola {nombreUsuario},</h3>
                <p>Se te ha asignado una nueva tarea: <strong>{tituloTarea}</strong>.</p>
                <p>Por favor, revisa el sistema para más detalles.</p>";

            return await EnviarCorreoAsync(correoDestino, "Nueva tarea asignada", cuerpoCorreo);
        }

        // 🔹 Nuevo: Notificar cambio de estado de tarea
        public async Task<bool> EnviarCorreoCambioEstadoTareaAsync(string correoDestino, string nombreUsuario, string tituloTarea, string nuevoEstado)
        {
            string cuerpoCorreo = $@"
                <h3>Hola {nombreUsuario},</h3>
                <p>El estado de la tarea <strong>{tituloTarea}</strong> ha cambiado a: <strong>{nuevoEstado}</strong>.</p>
                <p>Por favor, revisa el sistema para más detalles.</p>";

            return await EnviarCorreoAsync(correoDestino, "Cambio de estado de tarea", cuerpoCorreo);
        }

        // 🔹 Método para enviar correo
        private async Task<bool> EnviarCorreoAsync(string correoDestino, string asunto, string cuerpoCorreo)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Aviso de Proyecto de Lenguajes", "noreply@proyecto.com"));
                message.To.Add(new MailboxAddress("Destinatario", correoDestino));
                message.Subject = asunto;
                message.Body = new TextPart("html") { Text = cuerpoCorreo };

                using (var client = new SmtpClient())
                {
                    // Conectar al servidor SMTP
                    client.Connect("smtp.gmail.com", 587, false);

                    // Autenticación con la contraseña de la aplicación (debería ser segura)
                    client.Authenticate("tuemail@gmail.com", "tu_contrasena_de_aplicacion");

                    // Enviar el correo
                    await client.SendAsync(message);
                    client.Disconnect(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Manejar errores (log o notificación de error)
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
                return false;
            }
        }
    }
}


