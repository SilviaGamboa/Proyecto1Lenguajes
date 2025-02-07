using Microsoft.AspNetCore.Identity;

namespace ProyectoI.Repositories
{
    public class EncriptadorRepository : IEncriptadorRepository
    {
        private readonly PasswordHasher<object> _passwordHasher;

        // Constructor
        public EncriptadorRepository()
        {
            _passwordHasher = new PasswordHasher<object>();
        }

        // Encriptar contraseña
        public string Encriptar(string password)
        {
            return _passwordHasher.HashPassword(null, password);  // El primer parámetro (null) es un objeto genérico
        }

        // Verificar si una contraseña coincide con el hash
        public bool Verificar(string hashedPassword, string password)
        {
            return _passwordHasher.VerifyHashedPassword(null, hashedPassword, password) == PasswordVerificationResult.Success;
        }
    }
}
