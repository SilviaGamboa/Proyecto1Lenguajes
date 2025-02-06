
using ProyectoI.Models;
namespace ProyectoI.Repositories
{
    public interface ITareasRepository
    {
        List<TareaModel> GetAllTareas();
        TareaModel CreateTarea(TareaModel tarea);
        TareaModel UpdateTarea(TareaModel tarea);


        Boolean DeleteTareaByID(int id);

        Boolean AsignarUsuarioATarea(int tareaId, int usuarioId);

        Boolean CambiarEstadoTarea(int tareaId, string nuevoEstado);
        TareaModel GetTareaById(int tareaId);
    }
}
