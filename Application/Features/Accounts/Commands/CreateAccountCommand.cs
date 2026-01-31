using Application.Repositories;
using Common.Requests;
using Common.Wrapper;
using Domain;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Accounts.Commands
{
    public class CreateAccountCommand : IRequest<ResponseWrapper<int>>
    {
        public CreateAccountRequest CreateAccount { get; set; }

    }

    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, ResponseWrapper<int>>
    {
        private IUnitOfWork<int> _unitOfWork;
        private readonly ILogger<CreateAccountCommandHandler> _logger;
        public CreateAccountCommandHandler(IUnitOfWork<int> unitOfWork, ILogger<CreateAccountCommandHandler>logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ResponseWrapper<int>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling CreateAccountCommand for {@Request}", request.CreateAccount);
            //map incoming to domain account entity
            var account = request.CreateAccount.Adapt<Account>();
            //generate account number <> yyMMddHHmmss
            account.AccountNumber = AccountNumberGenerator.GenerateAccountNumber();
            _logger.LogInformation("Generated account number {AccountNumber}", account.AccountNumber);
            //activate account
            account.IsActive = true;
            //create account
            await _unitOfWork.WriteRepositoryFor<Account>().AddAsync(account);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Account created successfully with Id {Id}", account.Id);
            return new ResponseWrapper<int>().Success(data: account.Id, "Account created successfully");
        }
    }
}
