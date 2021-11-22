using BusinessLogic.Services;
using Domain.Models;
using Domain.Models.DTO;
using HOBBYNetMVC.Models.UserSettings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

        public UserSettingsController(UserManager<User> userManager, UserService userService, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
        }

        public IActionResult Index()
        {
            var output = _userManager.Users.Select(x => new UsersList(x.Email, x.Id)).ToList();
            return View(output.FirstOrDefault(u => u.Email == User.Identity.Name));
            //return View(_userManager.Users.FirstOrDefault(u => u.Email == User.Identity.Name));
        }

        [HttpGet]
        public async Task<IActionResult> Telegram(string id)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = await _userManager.FindByIdAsync(id);
            if (user == null || loginUserId != id)
            {
                return NotFound();
            }
            var model = new MessengerViewModel { Id = user.Id, MessengerUsername = user.TelegramUsername };
            return View(model);
        }

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
                        return RedirectToAction("Index");
                    
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Viber(string id)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = await _userManager.FindByIdAsync(id);
            if (user == null || loginUserId != id)
            {
                return NotFound();
            }
            var model = new MessengerViewModel { Id = user.Id, MessengerUsername = user.ViberUsername };
            return View(model);
        }

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
                    return RedirectToAction("Index");

                }
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> WhatsApp(string id)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = await _userManager.FindByIdAsync(id);
            if (user == null || loginUserId != id)
            {
                return NotFound();
            }
            var model = new MessengerViewModel { Id = user.Id, MessengerUsername = user.WhatsAppUsername };
            return View(model);
        }

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
                    return RedirectToAction("Index");

                }
            }
            return View(model);
        }




        [HttpGet]
        public async Task<IActionResult> ChangePassword(string id)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = await _userManager.FindByIdAsync(id);
            if (user == null || loginUserId != id)
            {
                return NotFound();
            }
            var model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
            return View(model);
        }

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
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePhoneNumber(string id)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = await _userManager.FindByIdAsync(id);
            if (user == null || loginUserId != id)
            {
                return NotFound();
            }
            var model = new ChangePhoneNumberViewModel { Id = user.Id, Email = user.Email, PhoneNumber = user.PhoneNumber };
            return View(model);
        }

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
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(model);
        }
    }
}
