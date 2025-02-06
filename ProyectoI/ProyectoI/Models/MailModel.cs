namespace ProyectoI.Models
{
    public class MailModel
    {
        public string From { get; set; } = "Administrador"; // Valor predeterminado
        public string To { get; set; }
        public string Subject { get; set; } = "Invitación al sistema"; // Valor predeterminado
        public string Body { get; set; } = "";
        
    }

}
