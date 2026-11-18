using System;
using System.Security.Cryptography;

namespace BTL_LapTrinhWeb.DAL
{
    internal static class PasswordHasher
    {
        private const int Iterations = 100000;

        public static string Hash(string password)
        {
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password is required.");
            var salt = new byte[16];
            using (var random = RandomNumberGenerator.Create()) random.GetBytes(salt);
            using (var derive = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                return "PBKDF2-SHA256$" + Iterations + "$" + Convert.ToBase64String(salt)
                    + "$" + Convert.ToBase64String(derive.GetBytes(32));
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(stored)) return false;
            var parts = stored.Split('$');
            int iterations;
            if (parts.Length != 4 || parts[0] != "PBKDF2-SHA256"
                || !int.TryParse(parts[1], out iterations) || iterations < 1 || iterations > 1000000)
                return false;
            try
            {
                var expected = Convert.FromBase64String(parts[3]);
                if (expected.Length != 32) return false;
                using (var derive = new Rfc2898DeriveBytes(password, Convert.FromBase64String(parts[2]),
                    iterations, HashAlgorithmName.SHA256))
                {
                    var actual = derive.GetBytes(32);
                    int difference = 0;
                    for (int i = 0; i < actual.Length; i++) difference |= actual[i] ^ expected[i];
                    return difference == 0;
                }
            }
            catch (FormatException) { return false; }
            catch (ArgumentException) { return false; }
        }
    }
}
