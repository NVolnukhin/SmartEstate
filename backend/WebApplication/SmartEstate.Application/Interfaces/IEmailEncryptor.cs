namespace SmartEstate.Application.Interfaces;

public interface IEmailEncryptor
{
    string Encrypt(string email);
    string Decrypt(string encryptedEmail); 
}