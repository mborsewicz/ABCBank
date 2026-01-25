using Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Requests
{
    public record CreateAccountRequest(string AccountNumber, decimal Balance, AccountType Type);


}
