using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using SuperDoorLocks.DomainModels;
using SuperDoorLocks.DomainModels.ServiceResultModels;

namespace SuperDoorLocks.Services
{
    public interface IDoorsService
    {
        Task<CreateActionOutcome> CreateDoor(string type, IEnumerable<string> roleNames);
        GetActionOutcome<Door> GetDoor(int id);
        ListActionOutcome<Door> ListDoors();
        AuthorizationRequiredActionOutcome OpenDoor(int id, ClaimsPrincipal user);
    }
}