using SmartEstate.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace SmartEstate.Infrastructure
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHashingSettings _settings;
    
        public PasswordHasher(IOptions<PasswordHashingSettings> settings)
        {
            _settings = settings.Value;
        }
    
        public string Generate(string clientHash) => 
            BCrypt.Net.BCrypt.EnhancedHashPassword(CombineWithServerSalt(clientHash));

        public bool Verify(string clientHash, string hashedPassword) =>
            BCrypt.Net.BCrypt.EnhancedVerify(CombineWithServerSalt(clientHash), hashedPassword);
    
        private string CombineWithServerSalt(string clientHash)
        {
            var serverSalt = string.Format(_settings.ServerSaltPattern, DateTime.Now.Year);
            return $"{clientHash}{serverSalt}";
        }
    }
}