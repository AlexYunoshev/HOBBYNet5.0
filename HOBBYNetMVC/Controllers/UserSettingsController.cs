using BusinessLogic.Services;
using Domain.Models;
using Domain.Models.DTO;
using HOBBYNetMVC.Models.UserSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HOBBYNetMVC.Controllers
{
    public class UserSettingsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UserService _userService;
        private readonly LocationService _locationService;
        private readonly IWebHostEnvironment _appEnvironment;

        public UserSettingsController(UserManager<User> userManager, IWebHostEnvironment appEnvironment, UserService userService, SignInManager<User> signInManager, LocationService locationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appEnvironment = appEnvironment;
            _userService = userService;
            _locationService = locationService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Telegram()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = await _userManager.FindByIdAsync(loginUserId);
            if (user == null)
            {
                return NotFound();
            }
            var model = new MessengerViewModel { Id = user.Id, MessengerUsername = user.TelegramUsername };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Telegram(MessengerViewModel model)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid && model.Id == loginUserId)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    _userService.SetTelegramUsername(loginUserId, model.MessengerUsername);
                    return RedirectToAction("Profile", "User");
                }
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Viber()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = await _userManager.FindByIdAsync(loginUserId);
            if (user == null)
            {
                return NotFound();
            }
            var model = new MessengerViewModel { Id = user.Id, MessengerUsername = user.ViberUsername };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Viber(MessengerViewModel model)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid && model.Id == loginUserId)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    _userService.SetViberUsername(loginUserId, model.MessengerUsername);
                    return RedirectToAction("Profile", "User");
                }
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> WhatsApp()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = await _userManager.FindByIdAsync(loginUserId);
            if (user == null)
            {
                return NotFound();
            }
            var model = new MessengerViewModel { Id = user.Id, MessengerUsername = user.WhatsAppUsername };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> WhatsApp(MessengerViewModel model)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid && model.Id == loginUserId)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    _userService.SetWhatsAppUsername(loginUserId, model.MessengerUsername);
                    return RedirectToAction("Profile", "User");
                }
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Location(int change = 0)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = await _userManager.FindByIdAsync(loginUserId);
            if (user == null)
            {
                return NotFound();
            }
            Location location = _locationService.GetUserLocation(loginUserId);
            if (change == 0)
            {
                return View(location);
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult LocationResultByGeolocation(string latitude, string longitude)
        {
            List<Location> locations = _locationService.GetLocations(latitude + " " + longitude);
            return View(locations);
        }

        [Authorize]
        [HttpPost]
        public IActionResult LocationResultByAddress(string address)
        {
            List<Location> locations = _locationService.GetLocations(address);
            return View(locations);
        }

        [Authorize]
        [HttpPost]
        public IActionResult SaveLocation(string latitude, string longitude, string name, long placeId, string address) { 
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Location location = new Location()
            {
                Latitude = latitude,
                Longitude = longitude,
                Name = name, 
                PlaceId = placeId, 
                Address = address
            };
            _locationService.SaveLocation(location, loginUserId);
            return View(location);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = await _userManager.FindByIdAsync(loginUserId);
            if (user == null)
            {
                return NotFound();
            }
            var model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid && model.Id == loginUserId)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    IdentityResult result =
                        await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Profile","User");
                    }
                }
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangePhoneNumber()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = await _userManager.FindByIdAsync(loginUserId);
            if (user == null)
            {
                return NotFound();
            }
            var model = new ChangePhoneNumberViewModel { Id = user.Id, Email = user.Email, PhoneNumber = user.PhoneNumber };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePhoneNumber(ChangePhoneNumberViewModel model)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid && model.Id == loginUserId)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    IdentityResult result = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Profile", "User");
                    }
                }
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangeUserPhoto()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User currentUser = _userManager.FindByIdAsync(loginUserId).Result;
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangeUserPhoto(IFormFile file)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User currentUser = _userManager.FindByIdAsync(loginUserId).Result;
            var rootPath = _appEnvironment.WebRootPath;
            await _userService.AddPhoto(currentUser.Id, file, rootPath);
            return RedirectToAction("Profile", "User");
        }
    }
}
