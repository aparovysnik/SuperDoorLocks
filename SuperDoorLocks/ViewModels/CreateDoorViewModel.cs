using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SuperDoorLocks.ViewModels
{
    public class CreateDoorViewModel
    {
        [Required]
        public string Type { get; set; }
        public List<string> PermittedRoles { get; set; }
    }
}
