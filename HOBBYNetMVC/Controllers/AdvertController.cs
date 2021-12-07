using BusinessLogic.Services;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOBBYNetMVC.Controllers
{
    public class AdvertController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UserService _userService;
        private readonly ExplorePostsService _explorePostsService;
        private readonly ILogger<UserController> _logger;
        private readonly HobbyService _hobbyService;


        public AdvertController(
            UserManager<User> userManager, SignInManager<User> signInManager,
            UserService userService, ExplorePostsService explorePostsService,
            HobbyService hobbyService,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _explorePostsService = explorePostsService;
            _hobbyService = hobbyService;
            _logger = logger;
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
