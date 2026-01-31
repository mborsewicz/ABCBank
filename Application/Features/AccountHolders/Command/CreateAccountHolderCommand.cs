using Common.Wrapper;
using Common.Requests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Repositories;
using Mapster;
using Domain;
using Microsoft.Extensions.Logging;

namespace Application.Features.AccountHolders.Command
{
    public class CreateAccountHolderCommand : IRequest<ResponseWrapper<int>>
    {
        public CreateAccountHolder CreateAccountHolder { get; set; }
    }

    public class CreateAccountHolderCommandHandler : IRequestHandler<CreateAccountHolderCommand, ResponseWrapper<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ILogger<CreateAccountHolderCommandHandler> _logger;
        public CreateAccountHolderCommandHandler(IUnitOfWork<int> unitOfWork, ILogger<CreateAccountHolderCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ResponseWrapper<int>> Handle(CreateAccountHolderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating account holder");
            var accountHolder = request.CreateAccountHolder.Adapt<AccountHolder>();
            
            await _unitOfWork.WriteRepositoryFor<AccountHolder>().AddAsync(accountHolder);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Account holder created with Id {Id}", accountHolder.Id);

            return new ResponseWrapper<int>().Success(accountHolder.Id, "Account holder created successfully.");
        }
    }
}
