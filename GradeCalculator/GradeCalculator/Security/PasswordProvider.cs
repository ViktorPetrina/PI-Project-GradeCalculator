using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace GradeCalculator.Security
{
    public class PasswordProvider
    {
        private static readonly Lazy<PasswordProvider> lazy = new Lazy<PasswordProvider>(() => new PasswordProvider());

        public static PasswordProvider Instance => lazy.Value;

        private PasswordProvider()
        {
        }

        public string GetSalt()
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            string b64Salt = Convert.ToBase64String(salt);

            return b64Salt;
        }

        public string GetHash(string password, string b64salt)
        {
            byte[] salt = Convert.FromBase64String(b64salt);

            byte[] hash =
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);
            string b64Hash = Convert.ToBase64String(hash);

            return b64Hash;
        }
    }
}
