using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTO
{
    public class UsersList
    {
        public UsersList(string id, string username, string photoPath)
        {
            Id = id;
            Username = username;
            PhotoPath = photoPath;
        }

        public string Id { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string PhotoPath { get; set; }

        public static FriendsDTO CurrentUserFriends { get; set; }
    }
}
