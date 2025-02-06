using ProyectoI.Models;
using Microsoft.Data.SqlClient;  // Asegúrate de importar esto


namespace ProyectoI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Método para autenticar un usuario por nombre y contraseña
        public async Task<UserModel> AuthenticateUserAsync(string correo, string contrasenna)
        {
            UserModel user = null;
            string query = "EXEC AutenticarUsuario @Correo, @Contrasenna"; // Llamamos al SP para autenticar al usuario
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Correo", correo);
                    command.Parameters.AddWithValue("@Contrasenna", contrasenna);

                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.Read()) // Si encontramos un usuario
                    {
                        user = new UserModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"]?.ToString(),
                            Correo = reader["Correo"]?.ToString(),
                            Contrasenna = reader["Contrasenna"]?.ToString()
                        };
                    }
                }
            }
            return user; // Si el usuario no es encontrado, retorna null
        }//fin metodo

        public async Task<bool> CreateUserAsync(string nombre, string correo, string contrasenna)
        {
            using SqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            string query = "EXEC CrearUsuario @Nombre, @Correo, @Contrasenna";

            using SqlCommand command = new(query, connection);

            command.Parameters.AddWithValue("@Nombre", nombre);
            command.Parameters.AddWithValue("@Correo", correo);
            command.Parameters.AddWithValue("@Contrasenna", contrasenna);

            int result = await command.ExecuteNonQueryAsync();

            return result > 0;
        }
    }//fin clase
}
