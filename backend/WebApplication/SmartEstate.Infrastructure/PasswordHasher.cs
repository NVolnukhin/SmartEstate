using SmartEstate.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace SmartEstate.Infrastructure
{
    public class PasswordHasher : IPasswordHasher
    {
        private const string ClientSalt = "fixed_client_salt_!@#";
        
        public string Generate(string clientHash) => 
            BCrypt.Net.BCrypt.EnhancedHashPassword(CombineWithServerSalt(clientHash));

        public bool Verify(string clientHash, string hashedPassword) =>
            BCrypt.Net.BCrypt.EnhancedVerify(CombineWithServerSalt(clientHash), hashedPassword);
        
        public string GenerateClientHash(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password + ClientSalt);
            var hashBytes = sha256.ComputeHash(bytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private string CombineWithServerSalt(string clientHash)
        {
            // В реальном проекте серверную соль нужно хранить для каждого пользователя отдельно
            // Это упрощенный пример с фиксированной солью
            return $"{clientHash}_server_salt_{DateTime.Now.Year}";
        }
    }
}