using System;
using System.Security.Cryptography;

namespace Domain.Globals
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            // Generate the hash
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);

            // Combine salt + hash
            byte[] hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);

            // Convert to base64
            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            // Convert the stored hash back to bytes
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Extract the salt from the stored hash
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Hash the provided password using the extracted salt
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);

            // Compare the hash of the provided password with the stored hash
            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    return false; // Passwords do not match
                }
            }

            return true; // Passwords match
        }
    }
}
