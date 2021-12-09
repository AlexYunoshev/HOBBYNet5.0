using BusinessLogic.Services;
using Domain.Models;
using Domain.ViewModels;
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
        private readonly HobbyService _hobbyService;

        public HobbyController(HobbyService hobbyService)
        {
            _hobbyService = hobbyService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userHobbies = _hobbyService.GetUserHobbiesList(loginUserId);
            var allHobbies = _hobbyService.GetAllHobbies();
            var viewModel = new HobbyViewModel();

            var hobbiesToAdd = new List<AddHobbiesModel>();
            foreach (var hobby in allHobbies)
            {
                if (!userHobbies.Contains(hobby))
                {
                    hobbiesToAdd.Add(new AddHobbiesModel() { Id = hobby.Id, Name = hobby.Name, IsSelected = false });
                }
                
            }
            viewModel.addHobbiesList = hobbiesToAdd;
            viewModel.userHobbiesList = userHobbies;
            return View(viewModel);
        }


        [Authorize]
        [HttpPost]
        public IActionResult AddHobbies(HobbyViewModel hobbiesModel)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hobbies = new List<Hobby>();
            foreach (var hobby in hobbiesModel.addHobbiesList)
            {
                if (hobby.IsSelected)
                {
                    hobbies.Add(new Hobby() { Id = hobby.Id, Name = hobby.Name });
                }  
            }
            _hobbyService.AddHobbiesToUser(loginUserId, hobbies);
            return RedirectToAction("Index");

        }

        [Authorize]
        [HttpPost]
        public IActionResult RemoveHobbyFromList(int hobbyId)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (_hobbyService.RemoveHobbyFromList(loginUserId, hobbyId) == false) return View("~/Views/Shared/ErrorPage.cshtml");
            return RedirectToAction("Index");
        }
    }
}
