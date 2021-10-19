using Domain.Interfaces;
using System.Collections.Generic;


namespace Domain.Models
{
    public class MainFeed : IEntity<int>
    {
        public int Id { get; set; }
        public List<Post> PostsList { get; set; }
    }
}
