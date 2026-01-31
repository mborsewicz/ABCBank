using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Common.Wrapper;
using Common.Requests;
using Application.Repositories;
using Domain;
using Microsoft.Extensions.Logging;

namespace Application.Features.AccountHolders.Command
{
    public class UpdateAccountHolderCommand : IRequest<ResponseWrapper<int>>
    {
        public UpdateAccountHolder UpdateAccountHolder { get; set; }
    }

    public class UpdateAccountHolderCommandHandler : IRequestHandler<UpdateAccountHolderCommand, ResponseWrapper<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ILogger<CreateAccountHolderCommandHandler> _logger;
        public UpdateAccountHolderCommandHandler(IUnitOfWork<int> unitOfWork, ILogger<CreateAccountHolderCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ResponseWrapper<int>> Handle(UpdateAccountHolderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update account holder");
            var accountHolderInDb = await _unitOfWork.ReadRepositoryFor<AccountHolder>().GetByIdAsync(request.UpdateAccountHolder.Id);

            if (accountHolderInDb is not null)
            {
                //update
                var updatedAccountHolder = accountHolderInDb.Update(
                    request.UpdateAccountHolder.FirstName,
                    request.UpdateAccountHolder.LastName,
                    request.UpdateAccountHolder.ContactNumber,
                    request.UpdateAccountHolder.Email
                );

                await _unitOfWork.WriteRepositoryFor<AccountHolder>().UpdateAsync(updatedAccountHolder);
                await _unitOfWork.CommitAsync(cancellationToken);

                return new ResponseWrapper<int>().Success(updatedAccountHolder.Id, "Account holder updated successfully");
            }
            return new ResponseWrapper<int>().Failed("Account holder not found");

        }
    }
}
