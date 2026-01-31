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

namespace Application.Features.AccountHolders.Queries
{
    public class GetAccountHoldersQuery : IRequest<ResponseWrapper<List<AccountHolderResponse>>>
    {
    }

    public class GetAccountHoldersQueryHandler : IRequestHandler<GetAccountHoldersQuery, ResponseWrapper<List<AccountHolderResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ILogger<GetAccountHoldersQueryHandler> _logger;
        public GetAccountHoldersQueryHandler(IUnitOfWork<int> unitOfWork, ILogger<GetAccountHoldersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ResponseWrapper<List<AccountHolderResponse>>> Handle(GetAccountHoldersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAccountHoldersQuery");

            var accountHoldersInDb = await _unitOfWork.ReadRepositoryFor<AccountHolder>().GetAllAsync();

            if(accountHoldersInDb.Count > 0)
            {
                var response = accountHoldersInDb.Adapt<List<AccountHolderResponse>>();
                _logger.LogInformation("Retrieved {Count} account holders", response.Count);
                return new ResponseWrapper<List<AccountHolderResponse>>().Success(response);
            }
            _logger.LogWarning("No account holders were found");
            return new ResponseWrapper<List<AccountHolderResponse>>().Failed("No account holders were found");
        }
    }
}
