using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Contracts
{
    public interface IEntity<TId> : IEntity
    {
        TId Id { get; set; }
    }
    public interface IEntity
    {
    }
}
