using Common.Wrapper;
using Common.Requests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Repositories;
using Mapster;
using Domain;

namespace Application.Features.AccountHolders.Command
{
    public class CreateAccountHolderCommand : IRequest<ResponseWrapper<int>>
    {
        public CreateAccountHolder CreateAccountHolder { get; set; }
    }

    public class CreateAccountHolderCommandHandler : IRequestHandler<CreateAccountHolderCommand, ResponseWrapper<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        public CreateAccountHolderCommandHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseWrapper<int>> Handle(CreateAccountHolderCommand request, CancellationToken cancellationToken)
        {
            var accountHolder = request.CreateAccountHolder.Adapt<AccountHolder>();
            
            await _unitOfWork.WriteRepositoryFor<AccountHolder>().AddAsync(accountHolder);
            await _unitOfWork.CommitAsync(cancellationToken);
            return new ResponseWrapper<int>().Success(accountHolder.Id, "Account holder created successfully.");
        }
    }
}
