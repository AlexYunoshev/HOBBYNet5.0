using Domain.Interfaces;
using System.Collections.Generic;

namespace Domain.Models
{
    public class Hobby : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<User> Users { get; set; } = new List<User>();
        public List<ExplorePost> ExplorePosts { get; set; } = new List<ExplorePost>();
        public List<Advert> Adverts { get; set; } = new List<Advert>();

        //public int UserId { get; set; }
        //public User User { get; set; }
    }
}
