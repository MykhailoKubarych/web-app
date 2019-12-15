using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;

namespace WebShop.Api.Domain
{
    public partial class User : BaseEntity
    {
        protected User()
        { }

        private User(string email, string salt, string passwordHash, Role role, string name = null)
        {
            Email = email;
            Salt = salt;
            PasswordHash = passwordHash;
            Role = role;
            Name = name;
        }
        
        public string? Name { get; protected set; }
        public string Email { get; protected set; }
        public Role Role { get; protected set; }
        public string Salt { get; protected set; }
        public string PasswordHash { get; protected set; }

        public static User Create(string email, string salt, string passwordHash, Role role)
        {
            Guard.Against.NullOrEmpty(email, nameof(email));
            Guard.Against.NullOrEmpty(salt, nameof(salt));
            Guard.Against.NullOrEmpty(passwordHash, nameof(passwordHash));
            
            return new User(email, salt, passwordHash, role);
        }

        public void UpdateName(string value)
        {
            Name = value;
        }
    }

    public enum Role : int
    {
        Customer
    }
}