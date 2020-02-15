using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebShop.Api.Features.User.Authenticate;
using WebShop.Api.Features.User.Register;

namespace WebShop.Api.Controllers
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost("sign-in")]
        public async Task<ActionResult<AuthResult>> AuthenticateAsync([FromBody] Authenticate credentials)
        {
            return Ok(await _mediator.Send(new AuthenticateQuery {Payload = credentials}));
        }
        
        [HttpPost("sign-up")]
        public async Task<ActionResult> RegisterAsync([FromBody] RegisterUser userCredentials)
        {
            await _mediator.Send(new RegisterUserCommand {Payload = userCredentials});
            return Ok();
        }
    }
}