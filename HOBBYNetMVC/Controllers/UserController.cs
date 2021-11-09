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
         
            //var users = _context.Users.ToList();
            //var friendsList = _context.FriendsList.Include(x => x.FriendUser).Where(x => x.MainUserId == loginUserId || x.FriendUserId == loginUserId).ToList();
            //var friendsList0 = _context.FriendsList.Include(x => x.FriendUser).ToList();

            var mainUsers = _context.FriendsList.Include(x => x.MainUser).Where(x => x.FriendUserId == loginUserId).ToList();
            var friendUsers = _context.FriendsList.Include(x => x.FriendUser).Where(x => x.MainUserId == loginUserId).ToList();
            var friendsList = mainUsers.Select(x => new FriendsList(x.MainUser.FirstName, x.MainUser.LastName, x.MainUserId)).ToList();
            friendsList.AddRange(friendUsers.Select(x => new FriendsList(x.FriendUser.FirstName, x.FriendUser.LastName, x.FriendUserId)).ToList());
            return View(friendsList);
        }


        [HttpGet]
        public async Task<IActionResult> Friends(string id)
        {
            //var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //User user = await _userManager.FindByIdAsync(id);
            //if (user == null || loginUserId != id)
            //{
            //    return NotFound();
            //}
            //var model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
            return View();
        }

       
    }
}
