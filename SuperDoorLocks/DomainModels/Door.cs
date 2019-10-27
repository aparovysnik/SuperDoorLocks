using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SuperDoorLocks.DomainModels
{
    public class Door
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public List<IdentityRole> PermittedRoles { get; set; }
        public bool IsOpen { get; set; }
    }
}
