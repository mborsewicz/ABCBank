using Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Requests
{
    public record CreateAccountRequest(int AccountHolderId, decimal Balance, AccountType Type);


}
