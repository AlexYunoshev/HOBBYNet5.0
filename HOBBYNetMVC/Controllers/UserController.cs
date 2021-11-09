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

        [HttpPost]
        public IActionResult Friends(string userId)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (loginUserId == null)
            {
                return View("~/Views/Shared/ErrorPage.cshtml");
            }

            var friends = _context.FriendsList.Include(x => x.FriendUser).Where(x => x.MainUserId == userId && x.FriendUserId == loginUserId).FirstOrDefault();
            if (friends == null) {
                friends = _context.FriendsList.Include(x => x.MainUser).Where(x => x.FriendUserId == userId && x.MainUserId == loginUserId).FirstOrDefault();
            }
            if (friends == null)
            {
                return View("~/Views/Shared/ErrorPage.cshtml");
            }
            _context.Remove(friends);
            _context.SaveChanges();

            return RedirectToAction("Friends");

            //var mainUsers = _context.FriendsList.Include(x => x.MainUser).Where(x => x.FriendUserId == loginUserId && x.RelationShips == RelationShips.Friend).ToList();
            //var friendUsers = _context.FriendsList.Include(x => x.FriendUser).Where(x => x.MainUserId == loginUserId && x.RelationShips == RelationShips.Friend).ToList();
            //var friendsList = mainUsers.Select(x => new FriendsList(x.MainUser.FirstName, x.MainUser.LastName, x.MainUserId)).ToList();
            //friendsList.AddRange(friendUsers.Select(x => new FriendsList(x.FriendUser.FirstName, x.FriendUser.LastName, x.FriendUserId)).ToList());
            //return View(friendsList);
        }


        [HttpGet]
        public IActionResult FriendRequests()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (loginUserId == null)
            {
                return View("~/Views/Shared/ErrorPage.cshtml");
            }

            var mainUsers = _context.FriendsList.Include(x => x.MainUser).Where(x => x.FriendUserId == loginUserId && x.RelationShips == RelationShips.Waiting).ToList();
            var requestsToUser = mainUsers.Select(x => new FriendsList(x.MainUser.FirstName, x.MainUser.LastName, x.MainUserId)).ToList();

            var friendUsers = _context.FriendsList.Include(x => x.FriendUser).Where(x => x.MainUserId == loginUserId && x.RelationShips == RelationShips.Waiting).ToList();
            var requestsFromUser = friendUsers.Select(x => new FriendsList(x.FriendUser.FirstName, x.FriendUser.LastName, x.FriendUserId)).ToList();

            //var friendsRequestsList = friendUsers.Select(x => new FriendsList(x.FriendUser.FirstName, x.FriendUser.LastName, x.FriendUserId)).ToList();
            List<List<FriendsList>> friendsRequestsList = new List<List<FriendsList>>();
            friendsRequestsList.Add(requestsToUser);
            friendsRequestsList.Add(requestsFromUser);
            int count = friendsRequestsList[0].Count;
            return View(friendsRequestsList);
        }


        [HttpPost]
        public IActionResult FriendRequests(string userId)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (loginUserId == null)
            {
                return View("~/Views/Shared/ErrorPage.cshtml");
            }

            var friends = _context.FriendsList.Include(x => x.FriendUser).Where(x => x.MainUserId == userId && x.FriendUserId == loginUserId).First();
            friends.RelationShips = RelationShips.Friend;
            _context.SaveChanges();
            return RedirectToAction("Friends", "User");
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
