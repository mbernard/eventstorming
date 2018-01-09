using System.Collections.Generic;

namespace Infra
{
    public interface IAggregate
    {
        string Id { get; }
        void Hydrate(object command);
        IEnumerable<object> Execute(object command);
    }
}