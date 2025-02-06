namespace ProyectoI.Models
{
    public class TareaModel
    {
        public int Id { get; set; }

        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public DateTime? FechaVencimiento { get; set; }

        public string Prioridad { get; set; }

        public int UsuarioCreadorId { get; set; }

        public int? UsuarioAsignadoId { get; set; }

        public string Estado { get; set; }
    }
}
