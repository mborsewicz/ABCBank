using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class AccountHolder : Person
    {
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public List<Account> Accounts { get; set; }
        public AccountHolder Update(string firstname, string lastname, string contactNumber, string email) 
        {
            if (firstname is not null && FirstName?.Equals(firstname, StringComparison.CurrentCultureIgnoreCase)is not true) FirstName = firstname;
            if (lastname is not null && LastName?.Equals(lastname, StringComparison.CurrentCultureIgnoreCase)is not true) LastName = lastname;
            if (contactNumber is not null && ContactNumber?.Equals(contactNumber, StringComparison.CurrentCultureIgnoreCase)is not true) ContactNumber = contactNumber;
            if (email is not null && Email?.Equals(email, StringComparison.CurrentCultureIgnoreCase)is not true) Email = email;

            return this;
        }
    }
}
