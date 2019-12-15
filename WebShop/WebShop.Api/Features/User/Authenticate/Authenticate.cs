using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using FluentValidation;
using Lamar;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebShop.Api.Core;
using WebShop.Api.DataAccess;
using WebShop.Api.Domain;

namespace WebShop.Api.Features.User.Authenticate
{
    public class Authenticate
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AuthenticateQuery : IRequest<AuthResult>
    {
        public Authenticate Payload { get; set; }

        public class Validator : AbstractValidator<Authenticate>
        {
            public Validator()
            {
                RuleFor(x => x).NotNull();
                
                RuleFor(x => x.Email)
                    .EmailAddress()
                    .NotEmpty()
                    .When(x => x != null);
                
                RuleFor(x => x.Password)
                    .NotEmpty()
                    .When(x => x != null);
            }
        }
    }

    internal class AuthenticateQueryHandler : IRequestHandler<AuthenticateQuery, AuthResult>
    {
        private readonly IOptionsMonitor<AuthOptions> _optionsMonitor;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IPasswordProvider _passwordProvider;

        public AuthenticateQueryHandler(
            IOptionsMonitor<AuthOptions> optionsMonitor,
            ISqlConnectionFactory sqlConnectionFactory, 
            IPasswordProvider passwordProvider)
        {
            _optionsMonitor = optionsMonitor;
            _sqlConnectionFactory = sqlConnectionFactory;
            _passwordProvider = passwordProvider;
        }
        
        public async Task<AuthResult> Handle(AuthenticateQuery request, CancellationToken cancellationToken)
        {
            using var connection = _sqlConnectionFactory.GetOpenConnection();
            const string sql = "SELECT * FROM \"Users\" WHERE \"Email\" = @Email";
            var userWithEnteredEmail = await connection.QueryFirstOrDefaultAsync<Domain.User>(sql, 
                new {Email = request.Payload.Email.ToLower()}) ?? throw new UnauthorizedAccessException();

            var isAuthenticated = _passwordProvider.IsValid(request.Payload.Password,
                userWithEnteredEmail.PasswordHash, userWithEnteredEmail.Salt);

            if (!isAuthenticated)
            {
                throw new UnauthorizedAccessException();
            }

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddDays(7),
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userWithEnteredEmail.Email.ToString()),
                    new Claim(ClaimTypes.Role, Role.Customer.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, userWithEnteredEmail.Id.ToString())
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_optionsMonitor.CurrentValue.SecretKey)),
                    SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return new AuthResult
            {
                AccessToken = jwtTokenHandler.WriteToken(token),
            };
        }
    }

    public class AuthResult
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}