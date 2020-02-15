using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FluentValidation;
using MediatR;
using WebShop.Api.Core;
using WebShop.Api.DataAccess;
using WebShop.Api.Domain;

namespace WebShop.Api.Features.User.Register
{
    public class RegisterUser
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        
    }

    public class RegisterUserCommand : IRequest
    {
        public RegisterUser Payload { get; set; }
        
        public class Validator : AbstractValidator<RegisterUserCommand>
        {
            public Validator(ISqlConnectionFactory sqlConnectionFactory)
            {
                RuleFor(x => x.Payload != null);
                RuleFor(x => x.Payload.Email)
                    .EmailAddress()
                    .MustAsync(async (x, _) =>
                    {
                        var emailToCheck = x.ToLower();
                        var query = $"SELECT \"Email\" FROM \"Users\" WHERE \"Email\" = @mailToCheck;";
                        using var connection = sqlConnectionFactory.GetOpenConnection();
                        var existingEmail = await connection.QueryFirstOrDefaultAsync<string>(
                                query, new {mailToCheck = emailToCheck});
                        return existingEmail != emailToCheck;
                    })
                    .When(x => x.Payload != null);
                RuleFor(x => x.Payload.Password)
                    .MinimumLength(6)
                    .When(x => x.Payload != null);
            }
        }
    }
    
    internal class RegisterUserCommandHandler : AsyncRequestHandler<RegisterUserCommand>
    {
        private readonly ShopDbContext _dbContext;
        private readonly IPasswordProvider _passwordProvider;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public RegisterUserCommandHandler(ShopDbContext dbContext, IPasswordProvider passwordProvider, ISqlConnectionFactory sqlConnectionFactory)
        {
            _dbContext = dbContext;
            _passwordProvider = passwordProvider;
            _sqlConnectionFactory = sqlConnectionFactory;
        }
        
        protected override async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var (salt, passwordHash) = _passwordProvider.ComputeHashParams(request.Payload.Password);
            var user = Domain.User.Create(request.Payload.Email.ToLower(), salt, passwordHash, Role.Customer);
            //user.UpdateName("John");

//            using var connection = _sqlConnectionFactory.GetOpenConnection();
//            await connection.InsertAsync(user);

            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}