using Domain.Models;
using Domain.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HOBBYNetMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(IWebHostEnvironment appEnvironment, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _appEnvironment = appEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View("Login", new AccountViewModel { loginViewModel = new LoginViewModel(), currentView = "register" });
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
                var rootPath = _appEnvironment.WebRootPath;
                var PhotoPath = Path.Combine(rootPath, "images");
                PhotoPath = Path.Combine(PhotoPath, "userPic.png");
                int index = PhotoPath.IndexOf("images");
                PhotoPath = PhotoPath.Remove(0, index - 1);

                User user = new User {
                    FirstName = model.registerViewModel.FirstName, 
                    LastName = model.registerViewModel.LastName, 
                    Email = model.registerViewModel.Email, 
                    UserName = model.registerViewModel.UserName,
                    PhotoPath = PhotoPath
                };

                var userByUsername = await _userManager.FindByNameAsync(user.UserName);
                if (userByUsername != null)
                {
                    ModelState.AddModelError("", "This username is already taken");
                    model.currentView = "register";
                    return View("Login", model);
                }

                var userByEmail = await _userManager.FindByEmailAsync(user.Email);
                if (userByEmail != null)
                {
                    ModelState.AddModelError("", "This email is already taken");
                    model.currentView = "register";
                    return View("Login", model);
                }

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
            model.currentView = "register";
            return View("Login", model);
        }

        [HttpGet]
        public IActionResult Login(string error = null)
        {
            if (error != null)
            {
                ModelState.AddModelError("", error);
                return View(new AccountViewModel { loginViewModel = new LoginViewModel(), currentView = "login"});
            }
            return View(new AccountViewModel { loginViewModel = new LoginViewModel() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.loginViewModel.Username, model.loginViewModel.Password, false, false);
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
