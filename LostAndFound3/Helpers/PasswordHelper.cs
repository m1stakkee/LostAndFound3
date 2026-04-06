using Konscious.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LostAndFound3.Views
{
    public class PasswordHelper 
    {

        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int DegreeOfParallelism = 8;
        private const int Iterations = 4;
        private const int MemorySize = 1024 * 1024;

        public string HashPassword(string password)
        {

            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }


            byte[] hash = HashPassword(password, salt);


            var combinedBytes = new byte[salt.Length + hash.Length];
            Array.Copy(salt, 0, combinedBytes, 0, salt.Length);
            Array.Copy(hash, 0, combinedBytes, salt.Length, hash.Length);


            return Convert.ToBase64String(combinedBytes);
        }

        private byte[] HashPassword(string password, byte[] salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = DegreeOfParallelism,
                Iterations = Iterations,
                MemorySize = MemorySize
            };

            return argon2.GetBytes(HashSize);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {

            byte[] combinedBytes = Convert.FromBase64String(hashedPassword);


            byte[] salt = new byte[SaltSize];
            byte[] hash = new byte[HashSize];
            Array.Copy(combinedBytes, 0, salt, 0, SaltSize);
            Array.Copy(combinedBytes, SaltSize, hash, 0, HashSize);


            byte[] newHash = HashPassword(password, salt);


            return CompareHashes(hash, newHash);
        }

        public bool CompareHashes(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length)
                return false;

            for (int i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                    return false;
            }
            return true;
        }
    }
}