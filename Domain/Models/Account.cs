using Domain.Interfaces;
using System.Collections.Generic;

namespace Domain.Models
{
    public class Account : IEntity<int>
    {
        public int Id { get; set; }
        public List<Hobby> HobbyList { get; set; }
        public User User { get; set; }
        public Feed Feed { get; set; }
    }
}
