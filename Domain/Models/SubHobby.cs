using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class SubHobby
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int HobbyId { get; set; }
        public Hobby Hobby { get; set; }


        public List<User> Users { get; set; } = new List<User>();
    }
}
