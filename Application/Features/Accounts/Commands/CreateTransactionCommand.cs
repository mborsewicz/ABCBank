using Application.Repositories;
using Common.Enums;
using Common.Requests;
using Common.Wrapper;
using Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Application.Features.Accounts.Commands
{
    public class CreateTransactionCommand : IRequest<ResponseWrapper<int>>
    {
        public TransactionRequest Transaction { get; set; }
    }

    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, ResponseWrapper<int>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ILogger<CreateTransactionCommandHandler> _logger;

        public CreateTransactionCommandHandler(IUnitOfWork<int> unitOfWork, ILogger<CreateTransactionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ResponseWrapper<int>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            //find account
            _logger.LogInformation("Finding account with ID: {AccountId}", request.Transaction.AccountId);
            var accountInDb = await _unitOfWork.ReadRepositoryFor<Account>().GetByIdAsync(request.Transaction.AccountId);
            if (accountInDb is not null)
            {
                //know the transaction type          
                if (request.Transaction.Type == TransactionType.Withdrawal)
                {
                    //validate if nessesary
                    if (request.Transaction.Amount > accountInDb.Balance)
                    {
                        _logger.LogWarning("Insufficient balance for withdrawal on Account ID: {AccountId}", request.Transaction.AccountId);
                        return new ResponseWrapper<int>().Failed(message: "Withdrawal amount is higher than account balance");
                    }
                    //create transaction
                    _logger.LogInformation("Creating withdrawal transaction for Account ID: {AccountId}", request.Transaction.AccountId);
                    var transaction = new Transaction()
                    {
                        AccountId = accountInDb.Id,
                        Amount = request.Transaction.Amount,
                        Type = TransactionType.Withdrawal,
                        Date = DateTime.Now
                    };

                    //update account balance
                    _logger.LogInformation("Updating account balance for Account ID: {AccountId}", request.Transaction.AccountId);
                    accountInDb.Balance -= request.Transaction.Amount;
                    await _unitOfWork.WriteRepositoryFor<Transaction>().AddAsync(transaction);
                    await _unitOfWork.WriteRepositoryFor<Account>().UpdateAsync(accountInDb);
                    await _unitOfWork.CommitAsync(cancellationToken);

                    _logger.LogInformation("Withdrawal transaction completed for Account ID: {AccountId}", request.Transaction.AccountId);
                    return new ResponseWrapper<int>().Success(data: transaction.Id, message: "Withdrawal was successfully");
                }
                else if (request.Transaction.Type == TransactionType.Deposit)
                {
                    //create transaction
                    _logger.LogInformation("Creating deposit transaction for Account ID: {AccountId}", request.Transaction.AccountId);
                    var transaction = new Transaction()
                    {
                        AccountId = accountInDb.Id,
                        Amount = request.Transaction.Amount,
                        Type = TransactionType.Deposit,
                        Date = DateTime.Now
                    };
                    //update account balance
                    _logger.LogInformation("Updating account balance for Account ID: {AccountId}", request.Transaction.AccountId);
                    accountInDb.Balance += request.Transaction.Amount;
                    await _unitOfWork.WriteRepositoryFor<Transaction>().AddAsync(transaction);
                    await _unitOfWork.WriteRepositoryFor<Account>().UpdateAsync(accountInDb);
                    await _unitOfWork.CommitAsync(cancellationToken);

                    _logger.LogInformation("Deposit transaction completed for Account ID: {AccountId}", request.Transaction.AccountId);
                    return new ResponseWrapper<int>().Success(data: transaction.Id, message: "Deposit was successfully");
                }
            }
            _logger.LogWarning("Account with ID: {AccountId} does not exist", request.Transaction.AccountId);
            return new ResponseWrapper<int>().Failed(message: "Account does exist");
        }
    }
}
