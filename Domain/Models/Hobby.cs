using Domain.Interfaces;
using System.Collections.Generic;

namespace Domain.Models
{
    public class Hobby : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<SubHobby> SubHobbies { get; set; } = new List<SubHobby>();

        //public int UserId { get; set; }
        //public User User { get; set; }
    }
}
