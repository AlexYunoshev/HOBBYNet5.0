using Domain.Interfaces;

namespace Domain.Models
{
    public class Hobby : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
