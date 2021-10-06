using Domain.Interfaces;
using System.Collections.Generic;

namespace Domain.Models
{
    public class Feed : IEntity<int>
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Account Account { get; set; }
        public List<Post> PostsList { get; set; }
    }
}
