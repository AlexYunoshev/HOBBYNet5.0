using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Friends
    {
        public string MainUserId { get; set; }

        [ForeignKey("MainUserId")]
        public User MainUser { get; set; }

        public string FriendUserId { get; set; }

        [ForeignKey("FriendUserId")]
        public User FriendUser { get; set; }

        public RelationShips RelationShips { get; set; }
    }
}
