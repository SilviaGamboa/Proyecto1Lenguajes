using ProyectoI.Models;

namespace ProyectoI.Repositories
{
    public interface IEmailRepository
    {
        Task<bool> SendEmailAsync(MailModel model, string contrasennaGenerada);
        string GenerarContrasennaSegura();

        // Notificar asignación de tarea
        Task<bool> EnviarCorreoAsignacionTareaAsync(string correoDestino, string nombreUsuario, string tituloTarea);

        // Notificar cambio de estado de tarea
        Task<bool> EnviarCorreoCambioEstadoTareaAsync(string correoDestino, string nombreUsuario, string tituloTarea, string nuevoEstado);
    }

}

