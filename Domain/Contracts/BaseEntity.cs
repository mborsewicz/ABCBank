using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Contracts
{
    public class BaseEntity<TId> : IEntity<TId>
    {
        public TId Id { get; set; }
    }
}
