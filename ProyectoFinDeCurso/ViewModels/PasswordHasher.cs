
using System.Security.Cryptography;
namespace ProyectoFinDeCurso.ViewModels
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password, int iterations = 100_000) //incripta la contraseña, solicitando la contraseña, y el numero de iteraciones, que por defecto son 100.000
        {
            var salt = new byte[16]; 
            using (var rng = RandomNumberGenerator.Create())//genera numeros aleatorios, con lo que incriptaremos con la contraseña, impidiendo haber ataques con tablas de busqueda
                rng.GetBytes(salt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256)) //pbkdf2 es donde se almacenará nuestra contraseña incriptada, y donde la devolverá totalmente incriptada, sin que pueda ser devuelta la anterior contrasña
            {
                var subkey = pbkdf2.GetBytes(32); // 256-bit
                return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(subkey)}";//devuelve una palabra, que sería 100000.saltCombertidaABase64.ValidadciónComparadaConHashes
            }
        }
        public static bool VerifyPassword(string password, string stored) //obtiene como password la palabra enviada por el usuario, y a demás obtiene la contraseña incriptada, verificando si la palabra enviada por el usuario es la correcta
        {
            if (string.IsNullOrWhiteSpace(stored)) return false; //evita se la contraseña incriptada no está vacía
            var parts = stored.Split('.'); //separa la palabra en tres partes, teniendo como refencias los .
            if (parts.Length != 3) return false; //devuelve falso si parts no tiene 3 partes

            int iterations = int.Parse(parts[0]); //combierte la primera parte en entero, que de base sería 100000
            var salt = Convert.FromBase64String(parts[1]);//combierte el salt de base 64 a bytes
            var expectedSubkey = Convert.FromBase64String(parts[2]); // Combierte el hast original de base 64 a bytes

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256)) //combierte la contraseña obtenida del usuario ha hash
            {
                var actualSubkey = pbkdf2.GetBytes(expectedSubkey.Length); //lee si tiene el tamaño necesario
                return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey); //devuelve true si las contraseñas son iguales
            }
        }
    }
}
