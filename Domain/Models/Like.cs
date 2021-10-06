using Domain.Interfaces;

namespace Domain.Models
{
    public class Like : IEntity<int>
    {
        public int Id { get; set; }
        public User User { get; set; }
    }
}
