using Application.Repositories;
using Common.Responses;
using Common.Wrapper;
using Domain;
using Mapster;
using MediatR;
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
        public GetAccountByIdQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseWrapper<AccountResponse>> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            // Implementation to get account by Id goes here
            var accountInDb = await _unitOfWork.ReadRepositoryFor<Account>().GetByIdAsync(request.Id);
            if(accountInDb is not null)
            {
                return new ResponseWrapper<AccountResponse>().Success(data: accountInDb.Adapt<AccountResponse>());
            }
            return new ResponseWrapper<AccountResponse>().Failed("Account does not exist");
        }
    }
}
