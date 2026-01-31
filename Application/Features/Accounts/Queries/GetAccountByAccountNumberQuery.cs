using Application.Repositories;
using Common.Responses;
using Common.Wrapper;
using Domain;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Accounts.Queries
{
    public class GetAccountByAccountNumberQuery : IRequest<ResponseWrapper<AccountResponse>>
    {
        public string AccountNumber { get; set; }
    }

    public class GetAccountByAccountNumberQueryHandler : IRequestHandler<GetAccountByAccountNumberQuery, ResponseWrapper<AccountResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ILogger<GetAccountByAccountNumberQueryHandler> _logger;
        public GetAccountByAccountNumberQueryHandler(IUnitOfWork<int> unitOfWork, ILogger<GetAccountByAccountNumberQueryHandler>logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ResponseWrapper<AccountResponse>> Handle(GetAccountByAccountNumberQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAccountByAccountNumberQuery for AccountNumber {AccountNumber}", request.AccountNumber);

            //LINQ to get account by AccountNumber
            var accountInDb =  _unitOfWork.ReadRepositoryFor<Account>()
            .Entities
            .Where(account => account.AccountNumber == request.AccountNumber)
            .FirstOrDefault();

            if (accountInDb is not null)
            {
                var response = accountInDb.Adapt<AccountResponse>();
                _logger.LogInformation("Found account {@Response}", response);
                return await Task.FromResult(new ResponseWrapper<AccountResponse>().Success(data: response));
            }
            _logger.LogWarning("Account with AccountNumber {AccountNumber} does not exist", request.AccountNumber);
            return await Task.FromResult(new ResponseWrapper<AccountResponse>().Failed("Account does not exist"));

        }
    }
}
