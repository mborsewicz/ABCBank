using Application.Features.Accounts.Commands;
using Application.Features.Accounts.Queries;
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

            var response = await Sender.Send(new CreateAccountCommand() { CreateAccount = createAccountRequest });
            if (response.IsSuccessful)
            {
                _logger.LogInformation("Account created successfully with Id {Id}", response.Data);
                return Ok(response);
            }
            _logger.LogWarning("Failed to create account {@Response}", response);
            return BadRequest(response);
        }

        [HttpPost("transact")]
        public async Task<IActionResult> TransactAsync([FromBody] TransactionRequest transaction)
        {
            _logger.LogInformation("Received TransactAccount request {@Transaction}", transaction);
            var response = await Sender.Send(new CreateTransactionCommand() { Transaction = transaction });
            if (response.IsSuccessful)
            {
                _logger.LogInformation("Account transaction completed successfully {@Response}", response);
                return Ok(response);
            }
            _logger.LogWarning("Failed to complete account transaction {@Response}", response);
            return BadRequest(response);
        }

        [HttpGet("by-id/{id}")]
        public async Task<IActionResult> GetAccountByIdAsync(int id)
        {
            _logger.LogInformation("Received GetAccountById request for Id {Id}", id);
            var response = await Sender.Send(new GetAccountByIdQuery() { Id = id });
            if (response.IsSuccessful)
            {
                _logger.LogInformation("Account retrieved successfully {@Response}", response);
                return Ok(response);
            }
            _logger.LogWarning("Failed to retrieve account {@Response}", response);
            return NotFound(response);
        }

        [HttpGet("by-account-number/{accountNumber}")]
        public async Task<IActionResult> GetAccountByAccountNumberAsync(string accountNumber)
        {
            _logger.LogInformation("Received GetAccountByAccountNumber request for AccountNumber {AccountNumber}", accountNumber);
            var response = await Sender.Send(new GetAccountByAccountNumberQuery() { AccountNumber = accountNumber });
            if (response.IsSuccessful)
            {
                _logger.LogInformation("Account retrieved successfully {@Response}", response);
                return Ok(response);
            }
            _logger.LogWarning("Failed to retrieve account {@Response}", response);
            return NotFound(response);

        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAccountsAsync()
        {
            _logger.LogInformation("Received GetAccounts request");
            var response = await Sender.Send(new GetAccountsQuery());
            if (response.IsSuccessful)
            {
                _logger.LogInformation("Accounts retrieved successfully {@Response}", response);
                return Ok(response);
            }
            _logger.LogWarning("Failed to retrieve accounts {@Response}", response);
            return NotFound(response);
        }
    }
}
