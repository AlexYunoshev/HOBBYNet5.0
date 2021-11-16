using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HOBBYNetMVC.Controllers
{
    public class ExplorePostsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ExplorePostsService _explorePostsService;

        public ExplorePostsController(UserManager<User> userManager, SignInManager<User> signInManager, ExplorePostsService explorePostsService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _explorePostsService = explorePostsService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            //var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var hobbies = _hobbyService.GetUserHobbiesList(loginUserId);
            //return View(hobbies);
        }
    }
}
