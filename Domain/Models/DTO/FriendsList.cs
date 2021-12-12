using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTO
{

    public class FriendsDTO
    {
        public List<FriendsList> Friends { get; set; } = new List<FriendsList>();
        public List<FriendsList> RequestsToUser { get; set; } = new List<FriendsList>();
        public List<FriendsList> RequestsFromUser { get; set; } = new List<FriendsList>();
    }

    public class FriendsList
    {
        public FriendsList(string id, string firstName, string lastName, string userName, string photoPath)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            PhotoPath = photoPath;
        }

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string PhotoPath { get; set; }
    }
}
