using System.Collections.Generic;

namespace SuperDoorLocks.DomainModels.ServiceResultModels
{
    public class ListActionOutcome<T>: ServiceActionOutcome
    {
        public IEnumerable<T> Items { get; set; }
    }
}
