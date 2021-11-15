using DataAccess.Context;
using Domain.Models;
using Domain.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class UserService
    {
        private readonly HobbyNetContext _context;

        public UserService(HobbyNetContext context)
        {
            _context = context;
        }

        public List<User> GetUsersList(string currentUserId)
        {
            var users = _context.Users.ToList();
            //users.Remove(_context.Users.Where(x => x.Id == currentUserId).First());
            users.RemoveAll(x => x.Id == currentUserId);
            return users;
        }

        public List<FriendsList> GetFriendsList(string currentUserId)
        {
            var mainUsers = _context.FriendsList.Include(x => x.MainUser).Where(x => x.FriendUserId == currentUserId && x.RelationShips == RelationShips.Friend).ToList();
            var friendUsers = _context.FriendsList.Include(x => x.FriendUser).Where(x => x.MainUserId == currentUserId && x.RelationShips == RelationShips.Friend).ToList();
            var friendsList = mainUsers.Select(x => new FriendsList(x.MainUser.FirstName, x.MainUser.LastName, x.MainUserId)).ToList();
            friendsList.AddRange(friendUsers.Select(x => new FriendsList(x.FriendUser.FirstName, x.FriendUser.LastName, x.FriendUserId)).ToList());
            return friendsList;
        }

        public List<FriendsList> GetFriendRequestsToUser(string currentUserId)
        {
            var mainUsers = _context.FriendsList
                .Include(x => x.MainUser)
                .Where(x => x.FriendUserId == currentUserId && x.RelationShips == RelationShips.Waiting)
                .ToList();
            var requestsToUser = mainUsers
                .Select(x => new FriendsList(x.MainUser.FirstName, x.MainUser.LastName, x.MainUserId))
                .ToList();
            return requestsToUser;
        }

        public List<FriendsList> GetFriendRequestsFromUser(string currentUserId)
        {
            var friendUsers = _context.FriendsList
                .Include(x => x.FriendUser)
                .Where(x => x.MainUserId == currentUserId && x.RelationShips == RelationShips.Waiting)
                .ToList();
            var requestsFromUser = friendUsers
                .Select(x => new FriendsList(x.FriendUser.FirstName, x.FriendUser.LastName, x.FriendUserId))
                .ToList();
            return requestsFromUser;
        }

        public bool AcceptFriendRequest(string currentUserId, string friendUserId)
        {
            var friends = _context.FriendsList
                .Include(x => x.FriendUser)
                .Where(x => x.MainUserId == friendUserId && x.FriendUserId == currentUserId)
                .FirstOrDefault();
            if (friends == null) return false;
            friends.RelationShips = RelationShips.Friend;
            _context.SaveChanges();
            return true;
        }

        public bool SendFriendRequest(User currentUser, User friendUser)
        {
     
            Friends friends = new Friends { MainUser = currentUser, FriendUser = friendUser, RelationShips = RelationShips.Waiting };
            _context.FriendsList.Add(friends);
            _context.SaveChanges();
            return true;
        }

        public bool RemoveUserFromFriends(string currentUserId, string friendUserId)
        {
            var friends = _context.FriendsList.Include(x => x.FriendUser)
                .Where(x => x.MainUserId == friendUserId && x.FriendUserId == currentUserId)
                .FirstOrDefault();
            if (friends == null)
            {
                friends = _context.FriendsList.Include(x => x.MainUser)
                    .Where(x => x.FriendUserId == friendUserId && x.MainUserId == currentUserId)
                    .FirstOrDefault();
            }
            if (friends == null)
            {
                return false;
            }
            _context.Remove(friends);
            _context.SaveChanges();
            return true;
        }
    }
}
