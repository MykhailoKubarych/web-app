using System;
using System.Security.Cryptography;
using System.Text;
using Ardalis.GuardClauses;

namespace WebShop.Api.Core
{
    public interface IPasswordProvider
    {
        SecuredPasswordParams ComputeHashParams(string password);
        bool IsValid(string password, string passwordHash, string salt);
    }
    
    public class PasswordProvider : IPasswordProvider
    {
        public SecuredPasswordParams ComputeHashParams(string password)
        {
            Guard.Against.NullOrEmpty(password, nameof(password));
            
            using var sha256Provider = SHA256.Create();
            var salt = ComputeHash(sha256Provider, $"{Guid.NewGuid():N}");
            var passwordHash = ComputeHash(sha256Provider, password + salt);
            
            return new SecuredPasswordParams(salt, passwordHash);
        }

        public bool IsValid(string password, string passwordHash, string salt)
        {
            Guard.Against.NullOrEmpty(password, nameof(password));
            Guard.Against.NullOrEmpty(passwordHash, nameof(passwordHash));
            Guard.Against.NullOrEmpty(salt, nameof(salt));
            
            using var sha256Provider = SHA256.Create();
            return ComputeHash(sha256Provider,password + salt) == passwordHash;
        }
        
        private static string ComputeHash(HashAlgorithm sha256Alg, string value)
        {
            var bytesArray = sha256Alg.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(bytesArray);
        }
    }

    public class SecuredPasswordParams
    {
        public SecuredPasswordParams(string salt, string passwordHash)
        {
            Salt = salt;
            PasswordHash = passwordHash;
        }
        public string Salt { get;}
        public string PasswordHash { get; }

        public void Deconstruct(out string salt, out string passwordHash)
        {
            salt = Salt;
            passwordHash = PasswordHash;
        }
    }
    
    
}