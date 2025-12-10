using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Requests
{
    internal record CreateAccountHolder(string FirstName, string LastName, DateTime DateOfBirth,
        string ContactNumber, string Email );

    internal record UpdateAccountHolder(int Id, string FirstName, string LastName, DateTime DateOfBirth,
    string ContactNumber, string Email);
}
