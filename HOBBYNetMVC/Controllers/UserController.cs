using DataAccess.Context;
using Domain.Models;
using Domain.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HOBBYNetMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly HobbyNetContext _context;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, HobbyNetContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (loginUserId == null)
            {
                return View("~/Views/Shared/ErrorPage.cshtml");
            }

            //var users2 = _context.Users.Join(_context.FriendsList, u => u.Id, f => f.MainUserId, (u, f) => new 
            //{ Id = u.Id, FirstName = u.FirstName, LastName = u.LastName, Email = u.Email, Friend = f.FriendUser}).ToList();

            //var users3 = (from user in _context.Users
            //             join friendList in _context.FriendsList on user.Id equals friendList.MainUserId
            //             select new
            //             {
            //                 Id = user.Id,
            //                 FirstName = user.FirstName,
            //                 LastName = user.LastName,
            //                 Email = user.Email,
            //                 Friend = friendList.FriendUser
            //             }).ToList();
            //var users4 = (from user in _context.Users from f in _context.FriendsList select new { user, f }).ToList();

            var users = _context.Users.ToList();
            users.Remove(_context.Users.Where(x => x.Id == loginUserId).First());
            return View(users);
        }

        [HttpGet]
        public IActionResult Friends()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (loginUserId == null)
            {
                return View("~/Views/Shared/ErrorPage.cshtml");
            }

            var mainUsers = _context.FriendsList.Include(x => x.MainUser).Where(x => x.FriendUserId == loginUserId && x.RelationShips == RelationShips.Friend).ToList();
            var friendUsers = _context.FriendsList.Include(x => x.FriendUser).Where(x => x.MainUserId == loginUserId && x.RelationShips == RelationShips.Friend).ToList();
            var friendsList = mainUsers.Select(x => new FriendsList(x.MainUser.FirstName, x.MainUser.LastName, x.MainUserId)).ToList();
            friendsList.AddRange(friendUsers.Select(x => new FriendsList(x.FriendUser.FirstName, x.FriendUser.LastName, x.FriendUserId)).ToList());
            return View(friendsList);
        }


        [HttpGet]
        public IActionResult FriendsRequests()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (loginUserId == null)
            {
                return View("~/Views/Shared/ErrorPage.cshtml");
            }

            var mainUsers = _context.FriendsList.Include(x => x.MainUser).Where(x => x.FriendUserId == loginUserId && x.RelationShips == RelationShips.Waiting).ToList();
            var friendUsers = _context.FriendsList.Include(x => x.FriendUser).Where(x => x.MainUserId == loginUserId && x.RelationShips == RelationShips.Waiting).ToList();
            var friendsRequestsList = mainUsers.Select(x => new FriendsList(x.MainUser.FirstName, x.MainUser.LastName, x.MainUserId)).ToList();
            //var friendsRequestsList = friendUsers.Select(x => new FriendsList(x.FriendUser.FirstName, x.FriendUser.LastName, x.FriendUserId)).ToList();
            return View(friendsRequestsList);
        }


        [HttpGet]
        public IActionResult AcceptFriendRequest(string id)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (loginUserId == null)
            {
                return View("~/Views/Shared/ErrorPage.cshtml");
            }

            var friends = _context.FriendsList.Include(x => x.FriendUser).Where(x => x.MainUserId == id && x.FriendUserId == loginUserId).First();
            friends.RelationShips = RelationShips.Friend;
            _context.SaveChanges();
            RedirectToAction("Friends", "User");
            //return View("~/Views/Shared/ErrorPage.cshtml");
   
            var mainUsers = _context.FriendsList.Include(x => x.MainUser).Where(x => x.FriendUserId == loginUserId && x.RelationShips == RelationShips.Waiting).ToList();
            var friendUsers = _context.FriendsList.Include(x => x.FriendUser).Where(x => x.MainUserId == loginUserId && x.RelationShips == RelationShips.Waiting).ToList();
            var friendsRequestsList = mainUsers.Select(x => new FriendsList(x.MainUser.FirstName, x.MainUser.LastName, x.MainUserId)).ToList();
            return View("Friends", friendsRequestsList);

        }


        
        [HttpGet]
        public async Task<IActionResult> AddFriendRequest(string id)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (loginUserId == null)
            {
                return View("~/Views/Shared/ErrorPage.cshtml");
            }

            User mainUser = await _userManager.FindByIdAsync(loginUserId);
            User friendUser = await _userManager.FindByIdAsync(id);
            Friends friends = new Friends { MainUser = mainUser, FriendUser = friendUser , RelationShips = RelationShips.Waiting};
            _context.FriendsList.Add(friends);
            _context.SaveChanges();
            var users = _context.Users.ToList();
            users.Remove(_context.Users.Where(x => x.Id == loginUserId).First());
            return View("Index",users);  
        }


    }
}
