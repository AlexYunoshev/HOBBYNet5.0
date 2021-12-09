using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geolocation;
using Microsoft.AspNetCore.Mvc;

namespace Domain.ViewModels
{
    public class ExplorePostsViewModel
    {
        public List<ExplorePost> Posts { get; set; }
        public List<ExplorePost> PostsByPage { get; set; }
        public int AllPostsCount { get; set; }
        public int PagesCount { get; set; }
        public int CurrentPageNumber { get; set; }
        public User CurrentUser { get; set; }
        public Dictionary<User, double> distanceToUser = new Dictionary<User, double>();

        public string AuthorizedUserId { get; set; }

        public int UserFriendsCount { get; set; }

        public bool IsCurrentUserPage = false;

        [BindProperty]
        public int LocationRadius { get; set; }


        [BindProperty]
        public string Sort { get; set; }
      

        public List<ExplorePost> GetPostsByPage(List<ExplorePost> allPosts, int pageNumber)
        {
            var postsOut = new List<ExplorePost>();
            int fullPagesCount = allPosts.Count / 10;
            int startIndex = (pageNumber - 1) * 10;
            int count = 10;
            if (pageNumber > fullPagesCount)
            {
                startIndex = fullPagesCount * 10;
                count = allPosts.Count - startIndex;
            }
            postsOut.AddRange(allPosts.GetRange(startIndex, count));
            return postsOut;
        }


        public ExplorePostsViewModel(int allPostsCount, User user, List<ExplorePost> posts, int pageNumber, int locationRadius, string sort)
        {
            CurrentUser = user;

            foreach (var post in posts)
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

                        double distance = GeoCalculator.GetDistance(originCoordinate, destinationCoordinate, 2, DistanceUnit.Kilometers);
                        distanceToUser.Add(post.User, distance);
                    }
                }
            }

            if (locationRadius != 0)
            {
                foreach (var key in distanceToUser.Keys)
                {
                    if (distanceToUser[key] > locationRadius)
                    {
                        posts.RemoveAll(p => p.User == key);
                    }
                }
            }
            
            if(sort == "newest") { posts = posts.OrderByDescending(p => p.CreatedDate).ToList(); }
            else if(sort == "older") { posts = posts.OrderBy(p => p.CreatedDate).ToList(); }

            else if(sort == "distanceAsc") {
                distanceToUser = distanceToUser.OrderBy(d => d.Value).ToDictionary(d => d.Key, d => d.Value);
                var sortedPosts = new List<ExplorePost>();
                foreach (var userKey in distanceToUser.Keys)
                {
                    sortedPosts.AddRange(posts.Where(p => p.UserId == userKey.Id));
                }
                posts.Clear();
                posts.AddRange(sortedPosts);
            }

            else
            {
                distanceToUser = distanceToUser.OrderByDescending(d => d.Value).ToDictionary(d => d.Key, d => d.Value);
                var sortedPosts = new List<ExplorePost>();
                foreach (var userKey in distanceToUser.Keys)
                {
                    sortedPosts.AddRange(posts.Where(p => p.UserId == userKey.Id));
                }
                posts.Clear();
                posts.AddRange(sortedPosts);
            }


            PostsByPage = GetPostsByPage(posts, pageNumber);
            Posts = PostsByPage;
           
            AllPostsCount = allPostsCount;
            if (AllPostsCount % 10 == 0)
            {
                PagesCount = AllPostsCount / 10;
            }
            else
            {
                PagesCount = AllPostsCount / 10 + 1;
            }

        }

     


    }
}
