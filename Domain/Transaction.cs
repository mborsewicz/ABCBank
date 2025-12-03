using Common.Enums;
using Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Transaction : BaseEntity<int>
    {
        public int AccountId { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Account Account { get; set; }

    }
}
