using Domain.Contracts;
using System.Runtime.InteropServices;

namespace Domain
{
    public class Person : BaseEntity<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
