using ProyectoI.Models;

namespace ProyectoI.Repositories
{
    public interface IUserRepository
    {
        // Método para autenticar al usuario por nombre y contraseña
        Task<UserModel> AuthenticateUserAsync(string correo, string contrasenna);

        Task<bool> CreateUserAsync(string nombre, string correo, string contrasenna);
        List<UserModel> GetAllUsers();

        UserModel GetUserByID(int id);

        UserModel UpdateUser(UserModel user);



    }
}
