using Domain.Interfaces;
using Geolocation;

namespace Domain.Models
{
    public class Location : IEntity<int>
    {
        public int Id { get; set; }
        //public Coordinate Coordinate { get; set; }
        public double CoordinateLongitude { get; set; }
        public double CoordinateLatitude { get; set; }
        public string Name { get; set; }
    }
}
