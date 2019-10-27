using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SuperDoorLocks.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 8)]
        public string Password { get; set; }

        public List<string> Roles { get; set; }
    }
}
