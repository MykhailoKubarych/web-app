using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebShop.Api.Domain;

namespace WebShop.Api.DataAccess.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Email).IsRequired();
            builder.Property(x => x.Salt).IsRequired();
            builder.Property(x => x.PasswordHash).IsRequired();
            builder.Property(x => x.Role).IsRequired().HasConversion<string>();
            builder.Property(x => x.Name);
        }
    }
}