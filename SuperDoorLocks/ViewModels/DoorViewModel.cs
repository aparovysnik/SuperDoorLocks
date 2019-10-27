using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperDoorLocks.ViewModels
{
    public class DoorViewModel
    {
        public string Type { get; }
        public List<string> PermittedRoles { get; }
        public bool IsOpen { get; }

        public DoorViewModel(string type, bool isOpen, List<string> permittedRoles)
        {
            Type = type;
            PermittedRoles = permittedRoles;
            IsOpen = isOpen;
        }
    }
}
