using Domain.Contracts;
using Common.Enums;

namespace Domain
{
    public class Account : BaseEntity<int>
    {
        public string AccountNumber { get; set; }
        public int AccountHolderId { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public AccountType Type { get; set; }
        public AccountHolder AccountHolder { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
