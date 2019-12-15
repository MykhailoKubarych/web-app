using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShop.Api.Domain;
using WebShop.Api.Domain.Features.Register;
using WebShop.Api.Features.User.Get;

namespace WebShop.Api.Controllers
{
    [Route("api/users")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<IEnumerable<User>>> GetAsync()
        {
            return Ok(await _mediator.Send(new GetUsersQuery()));
        }
    }
}