using System.Collections.Generic;
using System.Linq;

namespace SuperDoorLocks.DomainModels.ServiceResultModels
{
    public class ServiceActionOutcome
    {
        public virtual bool Success => !Errors.Any();

        public List<string> Errors { get; set; } = new List<string>();
    }
}