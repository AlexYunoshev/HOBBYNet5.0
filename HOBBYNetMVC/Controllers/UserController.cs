using BusinessLogic.Services;
using DataAccess.Context;
using Domain.Models;
using Domain.Models.DTO;
using HOBBYNetMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HOBBYNetMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UserService _userService;

        private readonly ILogger<UserController> _logger;

       

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, UserService userService, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        [HttpGet]
        public IActionResult UsersList()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(_userService.GetUsersList(loginUserId));
        }

        [Authorize]
        [HttpGet]
        public IActionResult Friends()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(_userService.GetFriendsList(loginUserId));
        }

        [Authorize]
        [HttpPost]
        public IActionResult Friends(string userId)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!_userService.RemoveUserFromFriends(loginUserId, userId))
            {
                return View("~/Views/Shared/ErrorPage.cshtml");
            }
            return RedirectToAction("Friends");
        }

        [Authorize]
        [HttpGet]
        public IActionResult FriendRequests()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var friendsRequestsList = new List<List<FriendsList>>();
            friendsRequestsList.Add(_userService.GetFriendRequestsToUser(loginUserId));
            friendsRequestsList.Add(_userService.GetFriendRequestsFromUser(loginUserId));
            return View(friendsRequestsList);
        }

        [Authorize]
        [HttpPost]
        // accept or decline friend request to me
        public IActionResult FriendRequests(string acceptUserId, string declineUserId)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(acceptUserId != null)
            {
                if(_userService.AcceptFriendRequest(loginUserId, acceptUserId) == false) return View("~/Views/Shared/ErrorPage.cshtml");
                return RedirectToAction("Friends", "User");
            }
            else
            {
                if (_userService.RemoveUserFromFriends(loginUserId, declineUserId) == false) return View("~/Views/Shared/ErrorPage.cshtml");
                return RedirectToAction("FriendRequests", "User");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AddFriendRequest(string id)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User mainUser = await _userManager.FindByIdAsync(loginUserId);
            User friendUser = await _userManager.FindByIdAsync(id);
            if (_userService.SendFriendRequest(mainUser, friendUser) == false) return View("~/Views/Shared/ErrorPage.cshtml");
            return RedirectToAction("FriendRequests");
        }
    }
}
