using Application.Features.AccountHolders.Command;
using Application.Features.AccountHolders.Queries;
using Common.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AccountHoldersController : BaseApiController
    {
        private readonly ILogger<AccountHoldersController> _logger;
        public AccountHoldersController(ILogger<AccountHoldersController> logger)
        {
            _logger = logger;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddAccountHolderAsync([FromBody] CreateAccountHolder createAccountHolder)
        {
            _logger.LogInformation("Received AddAccountHolder request {@Request}", createAccountHolder);

            var response = await Sender.Send(new CreateAccountHolderCommand { CreateAccountHolder = createAccountHolder });

            if (response.IsSuccessful)
            {
                _logger.LogInformation("Account holder created successfully with Id {Id}", response.Data);
                return Ok(response);

            }
            _logger.LogWarning("Failed to create account holder {@Response}", response);
            return BadRequest(response);

        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAccountHolderAsync([FromBody] UpdateAccountHolder updateAccountHolder)
        {
            _logger.LogInformation("Received UpdateAccountHolder request {@Request}", updateAccountHolder);

            var response = await Sender.Send(new UpdateAccountHolderCommand { UpdateAccountHolder = updateAccountHolder });
            if (response.IsSuccessful)
            {
                _logger.LogInformation("Account holder updated successfully with Id {Id}", updateAccountHolder.Id);
                return Ok(response);
            }
            _logger.LogWarning("Failed to update account holder {@Response}", response);
            return BadRequest(response);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccountHolderAsync(int id)
        {
            _logger.LogInformation("Received DeleteAccountHolder request for Id {Id}", id);

            var response = await Sender.Send(new DeleteAccountHolderCommand { Id = id });
            if (response.IsSuccessful)
            {
                _logger.LogInformation("Account holder deleted successfully with Id {Id}", id);
                return Ok(response);
            }
            _logger.LogWarning("Failed to delete account holder {@Response}", response);
            return BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountHolderByIdAsync(int id)
        {
            _logger.LogInformation("Received GetAccountHolderById request for Id {Id}", id);
            var response = await Sender.Send(new GetAccountHolderByIdQuery { Id = id });
            if (response.IsSuccessful)
            {
                _logger.LogInformation("Retrieved account holder with Id {Id}", id);
                return Ok(response);
            }
            _logger.LogWarning("Account holder not found for Id {Id}", id);
            return NotFound(response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAccountHoldersAsync()
        {
            _logger.LogInformation("Received GetAllAccountHolders request");
            var response = await Sender.Send(new GetAccountHoldersQuery());
            if (response.IsSuccessful)
            {
                _logger.LogInformation("Retrieved {Count} account holders", response.Data?.Count ?? 0);
                return Ok(response);
            }
            _logger.LogWarning("No account holders found");
            return NotFound(response);
        }
    }
}
