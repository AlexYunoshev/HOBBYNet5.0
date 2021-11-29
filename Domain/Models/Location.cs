using Domain.Interfaces;
using Geolocation;

namespace Domain.Models
{
    public class Location : IEntity<int>
    {
        public int Id { get; set; }
        public long PlaceId { get; set; }
        //public Coordinate Coordinate { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
