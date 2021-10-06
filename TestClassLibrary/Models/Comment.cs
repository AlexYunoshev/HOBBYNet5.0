using Domain.Interfaces;
using System.Collections.Generic;

namespace Domain.Models
{
    public class Comment : IEntity<int>
    {
        public int Id { get; set; }
        public Post Post { get; set; }
        public User User { get; set; }
        public string Text { get; set; }
        public List<Like> LikesList { get; set; }
    }
}
