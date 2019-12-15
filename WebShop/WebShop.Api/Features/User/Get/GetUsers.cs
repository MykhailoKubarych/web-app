using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using WebShop.Api.DataAccess;

namespace WebShop.Api.Features.User.Get
{
    public class GetUsersResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class GetUsersQuery : IRequest<IEnumerable<GetUsersResult>>
    {
    }
    
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<GetUsersResult>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetUsersQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }
        
        public async Task<IEnumerable<GetUsersResult>> Handle(GetUsersQuery request, CancellationToken cancellationToken = default)
        {
            const string getAllUsersSql = "SELECT \"Id\", \"Name\", \"Email\" FROM \"Users\"";
            using var connection = _sqlConnectionFactory.GetOpenConnection();
            return await connection.QueryAsync<GetUsersResult>(getAllUsersSql);
        }
    }
}