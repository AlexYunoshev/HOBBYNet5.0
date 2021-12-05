using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class ExplorePost : IEntity<int>
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string PhotoPath { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public List<Hobby> Hobbies { get; set; } = new List<Hobby>();

        public IList<ExploreLike> ExploreLikes { get; set; }
        public IList<ExploreComment> ExploreComments { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

    }
}
