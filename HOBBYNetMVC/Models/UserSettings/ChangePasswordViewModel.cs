using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HOBBYNetMVC.Models.UserSettings
{
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }

        [Required]
        [Display(Name = "Old password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} field  must have at least {2} symbols", MinimumLength = 8)]
        public string NewPassword { get; set; } 
    }
}
