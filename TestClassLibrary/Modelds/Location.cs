using Domain.Interfaces;
using Geolocation;

namespace Domain.Modelds
{
    public class Location : IEntity<int>
    {
        public int Id { get; set; }
        public Coordinate Longitude { get; set; }
        public Coordinate Latitude { get; set; }
        public string Name { get; set; }
    }
}
