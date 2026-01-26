using Application.Features.Accounts.Commands;
using Common.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : BaseApiController
    {
        [HttpPost("add")]
        public async Task<IActionResult> AddAccountAsync([FromBody] CreateAccountRequest createAccountRequest)
        {
            var response = await Sender.Send(new CreateAccountCommand { CreateAccount = createAccountRequest });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
