using SuperDoorLocks.DomainModels.ServiceResultModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperDoorLocks.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationOutcome> LoginUser(string username, string password);
        Task<AuthenticationOutcome> RegisterUser(string username, string password, IEnumerable<string> roles);
    }
}