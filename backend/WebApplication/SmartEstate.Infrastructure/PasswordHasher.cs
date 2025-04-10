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
        
        private string CombineWithServerSalt(string clientHash)
        {
            return $"{clientHash}_server_salt_{DateTime.Now.Year}";
        }
    }
}