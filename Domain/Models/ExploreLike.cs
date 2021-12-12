using Domain.Interfaces;

namespace Domain.Models
{
    public class ExploreLike
    {
        public string UserId { get; set; }
        public int PostId { get; set; }

        //[ForeignKey("UserId")]
        public User User { get; set; }

        //[ForeignKey("PostId")]
        public ExplorePost Post { get; set; }
    }
}
