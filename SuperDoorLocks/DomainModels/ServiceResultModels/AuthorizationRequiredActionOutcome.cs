using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperDoorLocks.DomainModels.ServiceResultModels
{
    public class AuthorizationRequiredActionOutcome: ServiceActionOutcome
    {
        public override bool Success => base.Success && !AuthErrors.Any();

        public List<string> AuthErrors { get; set; } = new List<string>();
    }
}
