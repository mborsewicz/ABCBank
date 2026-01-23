using Application.Features.AccountHolders.Command;
using Common.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AccountHoldersController : BaseApiController
    {
        [HttpPost("add")]
        public async Task<IActionResult> AddAccountHolderAsync([FromBody] CreateAccountHolder createAccountHolder)
        {
            var response = await Sender.Send(new CreateAccountHolderCommand { CreateAccountHolder = createAccountHolder });

            if (response.IsSuccessful)
            {
                return Ok(response);

            }
            return BadRequest(response);

        }
    }
}
