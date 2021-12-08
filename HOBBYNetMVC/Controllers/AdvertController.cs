using BusinessLogic.Services;
using Domain.Models;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly AdvertService _advertService;


        public AdvertController(
            UserManager<User> userManager, SignInManager<User> signInManager,
            UserService userService, ExplorePostsService explorePostsService,
            HobbyService hobbyService, AdvertService advertService,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _explorePostsService = explorePostsService;
            _hobbyService = hobbyService;
            _logger = logger;
            _advertService = advertService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            var adverts = _advertService.GetAllAdverts();
            return View(adverts);
        }

        [Authorize]
        [HttpGet]
        public IActionResult UserAdverts()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var adverts = _advertService.GetUserAdverts(loginUserId);
            return View(adverts);
        }

        [Authorize]
        [HttpPost]
        public IActionResult RemoveAdvert(int advertId)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _advertService.RemoveAdvert(loginUserId, advertId);
            return RedirectToAction("UserAdverts");
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddAdvert()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User currentUser = _userManager.FindByIdAsync(loginUserId).Result;


            var allHobbies = _hobbyService.GetUserHobbiesList(loginUserId);
            var viewModel = new HobbyViewModel();
            var hobbiesToAdd = new List<AddHobbiesModel>();

            foreach (var hobby in allHobbies)
            {
                hobbiesToAdd.Add(new AddHobbiesModel()
                {
                    Id = hobby.Id,
                    Name = hobby.Name,
                    IsSelected = false
                });

            }
            viewModel.addHobbiesList = hobbiesToAdd;

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddAdvert(string messageText, HobbyViewModel hobbiesModel)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User currentUser = _userManager.FindByIdAsync(loginUserId).Result;
            var hobbies = new List<Hobby>();
            foreach (var hobby in hobbiesModel.addHobbiesList)
            {
                if (hobby.IsSelected)
                {
                    hobbies.Add(new Hobby() { Id = hobby.Id, Name = hobby.Name });
                }
            }
            _advertService.AddAdvert(currentUser, messageText, hobbies);
            return RedirectToAction("UserAdverts");
        }
    }
}
