using Domain.Interfaces;
using System.Collections.Generic;

namespace Domain.Models
{
    public class ExploreComment : IEntity<int>
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public int PostId { get; set; }

        //[ForeignKey("UserId")]
        public User User { get; set; }

        //[ForeignKey("PostId")]
        public ExplorePost Post { get; set; }

        public string Text { get; set; }
    }
}
