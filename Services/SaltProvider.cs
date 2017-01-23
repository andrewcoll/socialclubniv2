using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace SocialClubNI.Services
{
    public class SaltProvider
    {
        /// <summary>
        /// Generate a new salt
        /// </summary>
        /// <returns>Base64 representation of salt</returns>
        public static string GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];

            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        /// <summary>
        /// Get a hashed password
        /// </summary>
        /// <param name="password">Plaintext password</param>
        /// <param name="existingSalt">Salt</param>
        /// <returns></returns>
        public static string GenerateHashedPassword(string password, string existingSalt)
        {
            var salt = Convert.FromBase64String(existingSalt);
            var saltedPassword = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA1, 10000, 256 / 8);

            return Convert.ToBase64String(saltedPassword);
        }
    }
}