using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geolocation;

namespace Domain.ViewModels
{
    public class ExplorePostsViewModel
    {
        public List<ExplorePost> Posts { get; set; }
        public int AllPostsCount { get; set; }
        public int PagesCount { get; set; }
        public int CurrentPageNumber { get; set; }
        public User CurrentUser { get; set; }
        public Dictionary<User, double> distanceToUser = new Dictionary<User, double>();
        
        public ExplorePostsViewModel(int allPostsCount, User user, List<ExplorePost> posts)
        {
            Posts = posts;
            CurrentUser = user;
            AllPostsCount = allPostsCount;
            if (AllPostsCount % 10 == 0)
            {
                PagesCount = AllPostsCount / 10;
            }
            else
            {
                PagesCount = AllPostsCount / 10 + 1;
            }

            foreach (var post in Posts)
            {
                if (post.User.Location != null)
                {
                    if (!distanceToUser.ContainsKey(post.User))
                    {
                        double originLatitude = Convert.ToDouble(CurrentUser.Location.Latitude);
                        double originLongitude = Convert.ToDouble(CurrentUser.Location.Longitude);

                        double destinationLatitude = Convert.ToDouble(post.User.Location.Latitude);
                        double destinationLongitude = Convert.ToDouble(post.User.Location.Longitude);

                        Coordinate originCoordinate = new Coordinate(originLatitude, originLongitude);
                        Coordinate destinationCoordinate = new Coordinate(destinationLatitude, destinationLongitude);

                        double distance = GeoCalculator.GetDistance(originCoordinate, destinationCoordinate, 3, DistanceUnit.Kilometers);
                        distanceToUser.Add(post.User, distance);
                    }
                }
               
            }
        }

     


    }
}
