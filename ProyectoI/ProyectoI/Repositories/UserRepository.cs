﻿using ProyectoI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProyectoI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Método para autenticar un usuario por correo
        public async Task<UserModel> AuthenticateUserAsync(string correo)
        {
            UserModel user = null;
            string query = "EXEC AuthenticateUserByEmail @Correo"; // Llamamos al SP para autenticar al usuario por correo

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Agregamos el parámetro de correo al comando
                    command.Parameters.AddWithValue("@Correo", correo);

                    // Ejecutamos el comando y leemos los resultados
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.Read()) // Si encontramos un usuario
                    {
                        // Asignamos los valores del usuario al modelo
                        user = new UserModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"]?.ToString(),
                            Correo = reader["Correo"]?.ToString(),
                            Contrasenna = reader["Contrasenna"]?.ToString() // La contraseña está encriptada en la base de datos
                        };
                    }
                }
            }

            return user; // Retorna el usuario encontrado o null si no se encuentra
        }//fin metodo

        public async Task<bool> CreateUserValidadoAsync(string nombre, string correo, string contrasenna)
        {
            using SqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            string query = "EXEC CrearUsuarioValidado @Nombre, @Correo, @Contrasenna, @Resultado OUTPUT";

            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@Nombre", nombre);
            command.Parameters.AddWithValue("@Correo", correo);
            command.Parameters.AddWithValue("@Contrasenna", contrasenna);

            // Parámetro de salida
            SqlParameter resultadoParam = new("@Resultado", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(resultadoParam);

            await command.ExecuteNonQueryAsync();

            int resultado = (int)resultadoParam.Value;

            return resultado == 1; // Devuelve true si el usuario fue creado, false si ya existía
        }

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

        public UserModel GetUserByCorreo(string correo)
        {
            UserModel user = null;
            string query = "EXEC ObtenerUsuarioPorCorreo @correo";

            using (SqlConnection connection = new(_connectionString))
            {
                connection.Open();
                using SqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@correo", correo);

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
