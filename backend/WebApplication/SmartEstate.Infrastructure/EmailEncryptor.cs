using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using SmartEstate.Application.Interfaces;

namespace SmartEstate.Infrastructure;

public class EmailEncryptor : IEmailEncryptor
{

    private readonly EmailEncryptionSettings _settings;

    public EmailEncryptor(IOptions<EmailEncryptionSettings> settings)
    {
        _settings = settings.Value;
    }

    public string Encrypt(string email)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_settings.EncryptionKey);
        aes.IV = Encoding.UTF8.GetBytes(_settings.IV);

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var emailBytes = Encoding.UTF8.GetBytes(email);

        byte[] encryptedBytes;
        using (var ms = new System.IO.MemoryStream())
        {
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(emailBytes, 0, emailBytes.Length);
                cs.FlushFinalBlock();
            }
            encryptedBytes = ms.ToArray();
        }

        return Convert.ToBase64String(encryptedBytes);
    }

    public string Decrypt(string encryptedEmail)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_settings.EncryptionKey);
        aes.IV = Encoding.UTF8.GetBytes(_settings.IV);

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        var encryptedBytes = Convert.FromBase64String(encryptedEmail);

        byte[] emailBytes;
        using (var ms = new System.IO.MemoryStream(encryptedBytes))
        {
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            {
                using (var sr = new System.IO.StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}