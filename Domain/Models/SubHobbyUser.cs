using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class SubHobbyUser
    {
        public int SubHobbiesId { get; set; }

        [ForeignKey("SubHobbiesId")]
        public SubHobby SubHobby { get; set; }

        public string UsersId { get; set; }

        [ForeignKey("UsersId")]
        public User User { get; set; }
    }
}
