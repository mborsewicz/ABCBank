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

namespace Application.Features.Accounts.Queries
{
    public class GetAccountTransactionQuery : IRequest<ResponseWrapper<List<TransactionResponse>>>
    {
        public int AccountId { get; set; }
    }

    public class GetAccountTransactionQueryHandler : IRequestHandler<GetAccountTransactionQuery, ResponseWrapper<List<TransactionResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ILogger<GetAccountTransactionQueryHandler> _logger;
        public GetAccountTransactionQueryHandler(IUnitOfWork<int>unitOfWork, ILogger<GetAccountTransactionQueryHandler>logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ResponseWrapper<List<TransactionResponse>>> Handle(GetAccountTransactionQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAccountTransactionQuery for AccountId: {AccountId}", request.AccountId);
            var transactionsInDb = _unitOfWork.ReadRepositoryFor<Transaction>()
                .Entities
                .Where(t => t.AccountId == request.AccountId)
                .ToList();

            if (transactionsInDb.Count > 0)
            {
                _logger.LogInformation("Found {Count} transactions for AccountId: {AccountId}", transactionsInDb.Count, request.AccountId);
                return await Task.FromResult(new ResponseWrapper<List<TransactionResponse>>().Success(data: transactionsInDb.Adapt<List<TransactionResponse>>()));
            }
            _logger.LogWarning("No transactions found for AccountId: {AccountId}", request.AccountId);
            return await Task.FromResult(new ResponseWrapper<List<TransactionResponse>>().Failed(message: "No transactions found for the specified account."));
        }
    }
}
