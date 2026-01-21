using Application.Repositories;
using Common.Responses;
using Common.Wrapper;
using Domain;
using Mapster;
using MediatR;
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
            public GetAccountHolderByIdQueryHandler(IUnitOfWork<int> unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public async Task<ResponseWrapper<AccountHolderResponse>> Handle(GetAccountHolderByIdQuery request, CancellationToken cancellationToken)
            {
               var accountHolderInDb = await _unitOfWork.ReadRepositoryFor<AccountHolder>().GetByIdAsync(request.Id);

                if (accountHolderInDb is not null)
                {
                     return new ResponseWrapper<AccountHolderResponse>().Success(accountHolderInDb.Adapt<AccountHolderResponse>());
                }
                return new ResponseWrapper<AccountHolderResponse>().Failed("No account holder found with the given id");
            }
        }
    }
}
