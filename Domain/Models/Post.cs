using Domain.Interfaces;
using System.Collections.Generic;

namespace Domain.Models
{
    public class Post : IEntity<int>
    {
        public int Id { get; set; }
        public Feed Feed { get; set; }
        public List<Hobby> HobbyTegsList { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public string PhotoPath { get; set; }
        public List<Like> LikesList { get; set; }
        public List<Comment> CommentsList { get; set; }
    }
}
