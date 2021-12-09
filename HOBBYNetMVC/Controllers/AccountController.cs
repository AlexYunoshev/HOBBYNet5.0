using Domain.Models;
using Domain.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOBBYNetMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User {
                    FirstName = model.registerViewModel.FirstName, 
                    LastName = model.registerViewModel.LastName, 
                    Email = model.registerViewModel.Email, 
                    UserName = model.registerViewModel.UserName 
                };
                var result = await _userManager.CreateAsync(user, model.registerViewModel.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Profile", "User");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            //ModelState.AddModelError("", "You entered incorrect data");
            model.currentView = "register";
            return View("Login", model);
            //return RedirectToAction("Login", model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new AccountViewModel { loginViewModel = new LoginViewModel() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.loginViewModel.Email, model.loginViewModel.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Profile", "User");   
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect username and/or password");
                }
            }
            model.currentView = "login";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
