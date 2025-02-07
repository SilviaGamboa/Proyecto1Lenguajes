namespace ProyectoI.Repositories
{
    public interface IEncriptadorRepository
    {
        string Encriptar(string password);  // Método para encriptar una contraseña
        bool Verificar(string hashedPassword, string password);  // Método para verificar una contraseña encriptada
    }
}
