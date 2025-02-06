using Microsoft.Data.SqlClient;
using ProyectoI.Models;

namespace ProyectoI.Repositories
{
    public class TareasRepository : ITareasRepository
    {
        private readonly string _connectionString;

        public TareasRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        //  Crear tarea
        public TareaModel CreateTarea(TareaModel tarea)
        {
            try
            {
                using SqlConnection connection = new(_connectionString);
                string query = "EXEC CrearTarea @titulo, @descripcion, @fecha_vencimiento, @prioridad, @usuario_creador_id, @usuario_asignado_id, @estado";

                using SqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@titulo", tarea.Titulo);
                command.Parameters.AddWithValue("@descripcion", tarea.Descripcion);
                command.Parameters.AddWithValue("@fecha_vencimiento", tarea.FechaVencimiento);
                command.Parameters.AddWithValue("@prioridad", tarea.Prioridad);
                command.Parameters.AddWithValue("@usuario_creador_id", tarea.UsuarioCreadorId);
                command.Parameters.AddWithValue("@usuario_asignado_id", tarea.UsuarioAsignadoId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@estado", tarea.Estado);

                connection.Open();
                command.ExecuteNonQuery();

                return tarea;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la tarea: " + ex.Message);
            }
        }

        //  Obtener todas las tareas
        public List<TareaModel> GetAllTareas()
        {
            List<TareaModel> tareas = new();

            try
            {
                using SqlConnection connection = new(_connectionString);
                string query = "EXEC ObtenerTareas";

                using SqlCommand command = new(query, connection);
                connection.Open();

                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tareas.Add(new TareaModel
                    {
                        Id = reader.GetInt32(0),
                        Titulo = reader.GetString(1),
                        Descripcion = reader.GetString(2),
                        FechaVencimiento = reader.GetDateTime(3),
                        Prioridad = reader.GetString(4),
                        UsuarioCreadorId = reader.GetInt32(5),
                        UsuarioAsignadoId = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                        Estado = reader.GetString(7)
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las tareas: " + ex.Message);
            }

            return tareas;
        }









        public TareaModel UpdateTarea(TareaModel tarea)
        {
            try
            {
                using SqlConnection connection = new(_connectionString);
                string query = "EXEC ActualizarTarea @id, @titulo, @descripcion, @fecha_vencimiento, @prioridad, @usuario_asignado_id, @estado";

                using SqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@id", tarea.Id);
                command.Parameters.AddWithValue("@titulo", tarea.Titulo);
                command.Parameters.AddWithValue("@descripcion", tarea.Descripcion);
                command.Parameters.AddWithValue("@fecha_vencimiento", tarea.FechaVencimiento);
                command.Parameters.AddWithValue("@prioridad", tarea.Prioridad);
                command.Parameters.AddWithValue("@usuario_asignado_id", tarea.UsuarioAsignadoId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@estado", tarea.Estado);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return tarea;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la tarea: " + ex.Message);
            }
        }

        public bool DeleteTareaByID(int id)
        {

            try
            {
                using SqlConnection connection = new(_connectionString);
                string query = "EXEC EliminarTarea @id";

                using SqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la tarea: " + ex.Message);
            }
        }

        public bool AsignarUsuarioATarea(int tareaId, int usuarioId)
        {
            try
            {
                using SqlConnection connection = new(_connectionString);
                string query = "EXEC AsignarUsuarioATarea @TareaID, @UsuarioID";

                using SqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@TareaID", tareaId);
                command.Parameters.AddWithValue("@UsuarioID", usuarioId);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al asignar usuario a la tarea: " + ex.Message);
            }
        }


        public bool CambiarEstadoTarea(int tareaId, string nuevoEstado)
        {
            try
            {
                using SqlConnection connection = new(_connectionString);
                string query = "EXEC CambiarEstadoTarea @TareaID, @NuevoEstado";

                using SqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@TareaID", tareaId);
                command.Parameters.AddWithValue("@NuevoEstado", nuevoEstado);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                throw new Exception("Error en la base de datos al cambiar el estado de la tarea: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error inesperado al cambiar el estado de la tarea: " + ex.Message);
            }
        }

        public TareaModel GetTareaById(int tareaId)
        {
            try
            {
                using SqlConnection connection = new(_connectionString);
                string query = "EXEC ObtenerTareaPorID @TareaID";

                using SqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@TareaID", tareaId);

                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new TareaModel
                    {
                        Id = reader.GetInt32(0),
                        Titulo = reader.GetString(1),
                        Descripcion = reader.GetString(2),
                        FechaVencimiento = reader.GetDateTime(3),
                        Prioridad = reader.GetString(4),
                        UsuarioCreadorId = reader.GetInt32(5),
                        UsuarioAsignadoId = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                        Estado = reader.GetString(7)
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la tarea: " + ex.Message);
            }
        }

    }
}
