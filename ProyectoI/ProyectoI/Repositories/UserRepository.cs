using ProyectoI.Models;
using Microsoft.Data.SqlClient;

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

        public List<UserModel> GetAllUsers()
        {
            var users = new List<UserModel>();
            string query = "EXEC ObtenerUsuarios";

            using (SqlConnection connection = new(_connectionString))
            {
                connection.Open();
                using SqlCommand command = new(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var user = new UserModel
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader["Nombre"]?.ToString() ?? string.Empty,
                        Correo = reader["Correo"]?.ToString() ?? string.Empty,
                        Contrasenna = ""
                    };
                    users.Add(user);
                }
            }
            return users;
        }

        public UserModel GetUserByID(int id)
        {
            UserModel user = null;
            string query = "EXEC ObtenerUsuarioPorId @id";

            using (SqlConnection connection = new(_connectionString))
            {
                connection.Open();
                using SqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        user = new UserModel
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader["Nombre"]?.ToString() ?? string.Empty,
                            Correo = reader["Correo"]?.ToString() ?? string.Empty,
                            Contrasenna = reader["Contrasenna"]?.ToString() ?? string.Empty
                        };
                    }
                }
            }

            return user;
        }

        //metodo para actualizar nombre y correo de usuario
        public UserModel UpdateUser(UserModel user)
        {
            using SqlConnection connection = new(_connectionString);
            string query = "EXEC ActualizarUsuario @id, @Nombre, @Correo";

            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@id", user.Id);
            command.Parameters.AddWithValue("@Nombre", user.Nombre);
            command.Parameters.AddWithValue("@Correo", user.Correo);
            connection.Open();
            command.ExecuteNonQuery();

            return user;
        }

    }//fin clase
}
