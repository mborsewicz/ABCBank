using Application.Repositories;
using Common.Requests;
using Common.Wrapper;
using Domain;
using Mapster;
using MediatR;
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
        public CreateAccountCommandHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseWrapper<int>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            //map incoming to domain account entity
            var account = request.CreateAccount.Adapt<Account>();
            //generate account number <> yyMMddHHmmss
            account.AccountNumber = AccountNumberGenerator.GenerateAccountNumber();
            //activate account
            account.IsActive = true;
            //create account
            await _unitOfWork.WriteRepositoryFor<Account>().AddAsync(account);
            await _unitOfWork.CommitAsync(cancellationToken);

            return new ResponseWrapper<int>().Success(data: account.Id, "Account created successfully");
        }
    }
}
