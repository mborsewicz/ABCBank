using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Accounts
{
    public static class AccountNumberGenerator
    {
        public static string GenerateAccountNumber()
        {
            return DateTime.Now.ToString("yyMMddHHmmss");
        }
    }
}
