using Domain.Interfaces;
using Geolocation;

namespace Domain.Models
{
    public class Location : IEntity<int>
    {
        public int Id { get; set; }
        //public Coordinate Coordinate { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public string Name { get; set; }
    }
}
