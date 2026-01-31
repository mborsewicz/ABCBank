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
    public class GetAccountsQuery : IRequest<ResponseWrapper<List<AccountResponse>>>
    {

    }

    public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, ResponseWrapper<List<AccountResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ILogger<GetAccountsQueryHandler> _logger;
        public GetAccountsQueryHandler(IUnitOfWork<int> unitOfWork, ILogger<GetAccountsQueryHandler>logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ResponseWrapper<List<AccountResponse>>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAccountsQuery");
            var accountsInDb = await _unitOfWork.ReadRepositoryFor<Account>().GetAllAsync();

            if (accountsInDb.Count > 0)
            {
                _logger.LogInformation("Accounts found in database: {Count}", accountsInDb.Count);
                var response = accountsInDb.Adapt<List<AccountResponse>>();               
                return new ResponseWrapper<List<AccountResponse>>().Success(data: response);
            }
            _logger.LogWarning("No accounts found in database");
            return new ResponseWrapper<List<AccountResponse>>().Failed("No accounts found");
        }
    }
}
