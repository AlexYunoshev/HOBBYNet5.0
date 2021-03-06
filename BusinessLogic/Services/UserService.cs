using DataAccess.Context;
using Domain.Models;
using Domain.Models.DTO;
using Domain.Models.Extentions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
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

        public List<UsersList> GetUsersBySearch(string searchString, string currentUserId)
        {
            var users = _context.Users
                .Where(u => u.UserName.Contains(searchString))
                .Select(x => new UsersList(x.Id, x.UserName, x.PhotoPath) {Firstname = x.FirstName, Lastname = x.LastName })
                .ToList();
            users.AddRange(_context.Users
                .Where(u => u.FirstName.Contains(searchString))
                .Select(x => new UsersList(x.Id, x.UserName, x.PhotoPath) { Firstname = x.FirstName, Lastname = x.LastName })
                .ToList());
            users.AddRange(_context.Users
                .Where(u => u.LastName.Contains(searchString))
                .Select(x => new UsersList(x.Id, x.UserName, x.PhotoPath) { Firstname = x.FirstName, Lastname = x.LastName })
                .ToList());
            users.RemoveAll(u => u.Id == currentUserId);
            return users.DistinctBy(u => u.Id).ToList();     
        }

        public User GetUserById(string userId)
        {
            var user = _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Location)
                .FirstOrDefault();
            return user;
        }

        public void SetTelegramUsername(string currentUserId, string telegramName)
        {
            var user = _context.Users.Where(u => u.Id == currentUserId).FirstOrDefault();
            user.TelegramUsername = telegramName;
            _context.SaveChanges();
        }

        public void SetViberUsername(string currentUserId, string viberName)
        {
            var user = _context.Users.Where(u => u.Id == currentUserId).FirstOrDefault();
            user.ViberUsername = viberName;
            _context.SaveChanges();
        }


        public void SetWhatsAppUsername(string currentUserId, string whatsAppName)
        {
            var user = _context.Users.Where(u => u.Id == currentUserId).FirstOrDefault();
            user.WhatsAppUsername = whatsAppName;
            _context.SaveChanges();
        }


        public List<User> GetUsersList(string currentUserId)
        {
            var users = _context.Users.ToList();
            users.RemoveAll(x => x.Id == currentUserId);
            return users;
        }

        public List<FriendsList> GetFriendsList(string currentUserId)
        {
            var mainUsers = _context.FriendsList
                .Include(x => x.MainUser)
                .Where(x => x.FriendUserId == currentUserId && x.RelationShips == RelationShips.Friend)
                .ToList();

            var friendUsers = _context.FriendsList
                .Include(x => x.FriendUser)
                .Where(x => x.MainUserId == currentUserId && x.RelationShips == RelationShips.Friend)
                .ToList();

            var friendsList = mainUsers
                .Select(x => new FriendsList(
                    x.MainUserId, 
                    x.MainUser.FirstName, 
                    x.MainUser.LastName,
                    x.MainUser.UserName,
                    x.MainUser.PhotoPath
                    )).ToList();

            friendsList
                .AddRange(friendUsers.Select(x => new FriendsList(
                    x.FriendUserId,
                    x.FriendUser.FirstName,
                    x.FriendUser.LastName, 
                    x.FriendUser.UserName,
                    x.FriendUser.PhotoPath
                    )).ToList());
            return friendsList;
        }

        public List<FriendsList> GetFriendRequestsToUser(string currentUserId)
        {
            var mainUsers = _context.FriendsList
                .Include(x => x.MainUser)
                .Where(x => x.FriendUserId == currentUserId && x.RelationShips == RelationShips.Waiting)
                .ToList();
            var requestsToUser = mainUsers
                .Select(x => new FriendsList(
                    x.MainUserId,
                    x.MainUser.FirstName,
                    x.MainUser.LastName,
                    x.MainUser.UserName,
                    x.MainUser.PhotoPath
                    )).ToList();
            return requestsToUser;
        }

        public List<FriendsList> GetFriendRequestsFromUser(string currentUserId)
        {
            var friendUsers = _context.FriendsList
                .Include(x => x.FriendUser)
                .Where(x => x.MainUserId == currentUserId && x.RelationShips == RelationShips.Waiting)
                .ToList();
            var requestsFromUser = friendUsers.Select(x => new FriendsList(
                    x.FriendUserId,
                    x.FriendUser.FirstName,
                    x.FriendUser.LastName,
                    x.FriendUser.UserName,
                    x.FriendUser.PhotoPath
                    )).ToList();
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


        public void CancelFriendRequest(User currentUser, User friendUser)
        {
            var request = _context.FriendsList
                .Where(m => m.MainUser == currentUser)
                .Where(f => f.FriendUser == friendUser)
                .Where(r => r.RelationShips == RelationShips.Waiting).FirstOrDefault();

            _context.FriendsList.Remove(request);
            _context.SaveChanges();
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
            _context.FriendsList.Remove(friends);
            _context.SaveChanges();
            return true;
        }

        public async Task AddPhoto(string userId, IFormFile file, string filePath)
        {
            var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null) return;

            if (file == null)
            {
                if (user.PhotoPath != null && !user.PhotoPath.EndsWith("userPic.png"))
                {
                    var deletePath = filePath + user.PhotoPath;
                    try
                    {
                        File.Delete(deletePath);
                    }
                    catch
                    {

                    }
                }

                filePath = Path.Combine(filePath, "images");
                filePath = Path.Combine(filePath, "userPic.png");
                int index = filePath.IndexOf("images");
                filePath = filePath.Remove(0, index - 1);
                user.PhotoPath = filePath;
                _context.SaveChanges();
                return;
            }

            filePath = Path.Combine(filePath, "images");
            filePath = Path.Combine(filePath, user.UserName);

            if (file.FileName.ToUpper().EndsWith(".JPG") || file.FileName.ToUpper().EndsWith(".PNG"))
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string fileName = "userPhoto";

                filePath = Path.Combine(filePath, fileName);
                if (file.FileName.ToUpper().EndsWith(".JPG"))
                {
                    filePath += ".jpg";
                }
                else if (file.FileName.ToUpper().EndsWith(".PNG"))
                {
                    filePath += ".png";
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                int index = filePath.IndexOf("images");
                filePath = filePath.Remove(0, index - 1);
                user.PhotoPath = filePath;
                _context.SaveChanges();
            }
        }
    }
}
