using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperDoorLocks.DomainModels.ServiceResultModels
{
    public class AuthenticationOutcome: ServiceActionOutcome
    {
        public string Token { get; set; }
    }
}
