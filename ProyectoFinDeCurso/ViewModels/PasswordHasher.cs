
using System.Security.Cryptography;
namespace ProyectoFinDeCurso.ViewModels
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password, int iterations = 100_000)
        {
            // Parámetros
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                var subkey = pbkdf2.GetBytes(32); // 256-bit
                return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(subkey)}";
            }
        }
        public static bool VerifyPassword(string password, string stored)
        {
            if (string.IsNullOrWhiteSpace(stored)) return false;
            var parts = stored.Split('.');
            if (parts.Length != 3) return false;

            int iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var expectedSubkey = Convert.FromBase64String(parts[2]);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                var actualSubkey = pbkdf2.GetBytes(expectedSubkey.Length);
                return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
            }
        }
    }
}
