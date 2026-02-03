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
    public class GetAccountHolderByIdQuery : IRequest<ResponseWrapper<AccountHolderResponse>>
    {
        public int Id { get; set; }
        public class GetAccountHolderByIdQueryHandler : IRequestHandler<GetAccountHolderByIdQuery, ResponseWrapper<AccountHolderResponse>>
        {
            private readonly IUnitOfWork<int> _unitOfWork;
            private readonly ILogger<GetAccountHolderByIdQueryHandler> _logger;
            public GetAccountHolderByIdQueryHandler(IUnitOfWork<int> unitOfWork, ILogger<GetAccountHolderByIdQueryHandler> logger)
            {
                _unitOfWork = unitOfWork;
                _logger = logger;
            }
            public async Task<ResponseWrapper<AccountHolderResponse>> Handle(GetAccountHolderByIdQuery request, CancellationToken cancellationToken)
            {
                _logger.LogInformation("Handling GetAccountHolderByIdQuery for Id {Id}", request.Id);

                var accountHolderInDb = await _unitOfWork.ReadRepositoryFor<AccountHolder>().GetByIdAsync(request.Id);

                if (accountHolderInDb is not null)
                {
                    var response = accountHolderInDb.Adapt<AccountHolderResponse>();
                    _logger.LogInformation("Found account holder {@Response}", response);
                    return new ResponseWrapper<AccountHolderResponse>().Success(response);
                }
                _logger.LogWarning("No account holder found with Id {Id}", request.Id);
                return new ResponseWrapper<AccountHolderResponse>().Failed($"No account holder found with the given id: {request.Id}");
            }
        }
    }
}
