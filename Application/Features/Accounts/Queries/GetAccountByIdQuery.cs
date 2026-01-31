using Application.Repositories;
using Common.Responses;
using Common.Wrapper;
using Domain;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Application.Features.Accounts.Queries
{
    public class GetAccountByIdQuery :IRequest<ResponseWrapper<AccountResponse>>
    {
        public int Id { get; set; }

    }

    public class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, ResponseWrapper<AccountResponse>>
    {
        private IUnitOfWork<int> _unitOfWork;
        private ILogger<GetAccountByIdQueryHandler> _logger;
        public GetAccountByIdQueryHandler(IUnitOfWork<int> unitOfWork, ILogger<GetAccountByIdQueryHandler>logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ResponseWrapper<AccountResponse>> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAccountByIdQuery for Id {Id}", request.Id);

            // Implementation to get account by Id goes here
            var accountInDb = await _unitOfWork.ReadRepositoryFor<Account>().GetByIdAsync(request.Id);
            if(accountInDb is not null)
            {
                var response = accountInDb.Adapt<AccountResponse>();
                _logger.LogInformation("Found account {@Response}", response);
                return new ResponseWrapper<AccountResponse>().Success(data: response);
            }
            _logger.LogWarning("Account with Id {Id} does not exist", request.Id);
            return new ResponseWrapper<AccountResponse>().Failed("Account does not exist");
        }
    }
}
