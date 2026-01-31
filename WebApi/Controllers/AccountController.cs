using Application.Features.Accounts.Commands;
using Common.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : BaseApiController
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddAccountAsync([FromBody] CreateAccountRequest createAccountRequest)
        {
            _logger.LogInformation("Received AddAccount request {@Request}", createAccountRequest);

            var response = await Sender.Send(new CreateAccountCommand { CreateAccount = createAccountRequest });
            if (response.IsSuccessful)
            {
                _logger.LogInformation("Account created successfully with Id {Id}", response.Data);
                return Ok(response);
            }
            _logger.LogWarning("Failed to create account {@Response}", response);
            return BadRequest(response);
        }
    }
}
