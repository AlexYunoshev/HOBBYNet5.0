using BusinessLogic.Services;
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
    public class HobbyController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly HobbyService _hobbyService;

        public HobbyController(UserManager<User> userManager, SignInManager<User> signInManager, HobbyService hobbyService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hobbyService = hobbyService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hobbies = _hobbyService.GetUserHobbiesList(loginUserId);
            return View(hobbies);
        }
    }
}
