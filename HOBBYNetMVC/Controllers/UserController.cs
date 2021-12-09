using BusinessLogic.Services;
using DataAccess.Context;
using Domain.Models;
using Domain.Models.DTO;
using Domain.ViewModels;
using HOBBYNetMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HOBBYNetMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UserService _userService;
        private readonly ExplorePostsService _explorePostsService;
        private readonly ILogger<UserController> _logger;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly HobbyService _hobbyService;


        public UserController(
            UserManager<User> userManager, SignInManager<User> signInManager, 
            UserService userService, ExplorePostsService explorePostsService,
            HobbyService hobbyService,
            ILogger<UserController> logger, IWebHostEnvironment appEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _explorePostsService = explorePostsService;
            _hobbyService = hobbyService;
            _logger = logger;
            _appEnvironment = appEnvironment;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Profile(int pageNumber = 1)
        {

            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var posts = _explorePostsService.GetExplorePostsByUser(loginUserId);
            //var postsByPage = _explorePostsService.GetPostsByPage(posts, pageNumber);
            User currentUser = _userService.GetUserById(loginUserId);
            var userFriendsCount = _userService.GetFriendsList(loginUserId).Count;
            var viewModele = new ExplorePostsViewModel(posts.Count, currentUser,
                posts, pageNumber, 0)
            { CurrentPageNumber = pageNumber, UserFriendsCount = userFriendsCount };
            return View(viewModele);
        }

        [Authorize]
        [HttpGet]
        public IActionResult ToUserProfile(string id, int pageNumber = 1)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == loginUserId) { return RedirectToAction("Profile"); }

            var posts = _explorePostsService.GetExplorePostsByUser(id);
            User currentUser = _userManager.FindByIdAsync(id).Result;

            var userFriendsCount = _userService.GetFriendsList(loginUserId).Count;
            var viewModele = new ExplorePostsViewModel(posts.Count, currentUser,
                posts, pageNumber, 0)
            { CurrentPageNumber = pageNumber, UserFriendsCount = userFriendsCount, AuthorizedUserId = loginUserId };
            return View(viewModele);
        }



        [Authorize]
        [HttpPost]
        public IActionResult RemovePost(int postId, int pageNumber)
        {
            
            var rootPath = _appEnvironment.WebRootPath;
           
            _explorePostsService.RemovePost(postId, rootPath);
            string url = "Profile?pageNumber=" + pageNumber;
            return Redirect(url);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddPost()
        {
           

            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User currentUser = _userManager.FindByIdAsync(loginUserId).Result;


            var allHobbies = _hobbyService.GetUserHobbiesList(loginUserId);
            var viewModel = new HobbyViewModel();
            var hobbiesToAdd = new List<AddHobbiesModel>();

            foreach (var hobby in allHobbies)
            {
                hobbiesToAdd.Add(new AddHobbiesModel() { 
                    Id = hobby.Id, 
                    Name = hobby.Name, 
                    IsSelected = false });

            }
            viewModel.addHobbiesList = hobbiesToAdd;
          
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddPost(string postText, IFormFile file, HobbyViewModel hobbiesModel)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User currentUser = _userManager.FindByIdAsync(loginUserId).Result;
            var rootPath = _appEnvironment.WebRootPath;
            var startFilePath = Path.Combine(rootPath, "images");
            startFilePath = Path.Combine(startFilePath, currentUser.UserName);

            var hobbies = new List<Hobby>();
            foreach (var hobby in hobbiesModel.addHobbiesList)
            {
                if (hobby.IsSelected)
                {
                    hobbies.Add(new Hobby() { Id = hobby.Id, Name = hobby.Name });
                }
            }

            await _explorePostsService.AddPost(currentUser, postText, file, startFilePath, hobbies);
            return RedirectToAction("Profile");
        }



        [Authorize]
        [HttpGet]
        public IActionResult EditPost(int postId, int pageNumber)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User currentUser = _userManager.FindByIdAsync(loginUserId).Result;

            var post = _explorePostsService.GetExplorePost(postId);
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

            foreach (var hobby in post.Hobbies)
            {
                hobbiesToAdd.Where(h => h.Name == hobby.Name).FirstOrDefault().IsSelected = true;
            }

            viewModel.addHobbiesList = hobbiesToAdd;



            var editPostViewModel = new EditPostViewModel();



            editPostViewModel.Post = post;
            editPostViewModel.HobbyViewModel = viewModel;

            return View(editPostViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> EditPost(int postId, string postText, IFormFile file, EditPostViewModel editPostViewModel)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User currentUser = _userManager.FindByIdAsync(loginUserId).Result;
            var rootPath = _appEnvironment.WebRootPath;
            var startFilePath = Path.Combine(rootPath, "images");
            startFilePath = Path.Combine(startFilePath, currentUser.UserName);

            var hobbies = new List<Hobby>();
            //var allUserHobbies = new List<Hobby>();
            var allUserHobbies = _hobbyService.GetUserHobbiesList(loginUserId);
            foreach (var hobby in editPostViewModel.HobbyViewModel.addHobbiesList)
            {
                if (hobby.IsSelected)
                {
                    hobbies.Add(new Hobby() { Id = hobby.Id, Name = hobby.Name });
                }
                //allUserHobbies.Add(new Hobby() { Id = hobby.Id, Name = hobby.Name });
            }

            await _explorePostsService.EditPost(postId, postText, file, startFilePath, hobbies, allUserHobbies);
            return RedirectToAction("Profile");
        }




        [Authorize]
        [HttpPost]
        public IActionResult SetLikeToPost(int postId, int pageNumber, string profileUserId = null)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _explorePostsService.SetLikeToPost(postId, loginUserId);
            string url;
            if (loginUserId == profileUserId || profileUserId == null)
            {
                url = "Profile?pageNumber=" + pageNumber + "#post-" + postId;
                return Redirect(url);
            }
            url = "ToUserProfile?id=" + profileUserId + "&pageNumber=" + pageNumber + "#post-" + postId;
            return Redirect(url);
        }


        [Authorize]
        [HttpPost]
        public IActionResult AddCommentToPost(string commentText, int postId, int pageNumber, string profileUserId = null)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _explorePostsService.AddCommentToPost(postId, loginUserId, commentText);
            var post = _explorePostsService.GetExplorePost(postId);
            string url;
            if (loginUserId == profileUserId || profileUserId == null)
            {
                url = "Profile?pageNumber=" + pageNumber + "#post-" + postId;
                return Redirect(url);
            }
            url = "ToUserProfile?id=" + profileUserId + "&pageNumber=" + pageNumber + "#post-" + postId;
            return Redirect(url);
            //return RedirectToAction("Profile");
            //return View("PostComments", post);
        }

        [Authorize]
        [HttpPost]
        public IActionResult RemoveCommentFromPost(int commentId, int postId, int pageNumber, string profileUserId = null)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = _explorePostsService.GetExplorePost(postId);
            _explorePostsService.RemoveCommentFromPost(postId, commentId);
            string url;
            if (loginUserId == profileUserId || profileUserId == null)
            {
                url = "Profile?pageNumber=" + pageNumber + "#post-" + postId;
                return Redirect(url);
            }
            url = "ToUserProfile?id=" + profileUserId + "&pageNumber=" + pageNumber + "#post-" + postId;
            return Redirect(url); ;
            //return View("PostComments", post);
        }







        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Authorize]
        [HttpGet]
        public IActionResult Friends()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var friendsList = _userService.GetFriendsList(loginUserId);
            FriendsDTO friendsDTO = new FriendsDTO();
            friendsDTO.Friends = friendsList;
            friendsDTO.RequestsToUser = _userService.GetFriendRequestsToUser(loginUserId);
            friendsDTO.RequestsFromUser = _userService.GetFriendRequestsFromUser(loginUserId);
            return View(friendsDTO);
        }

        [Authorize]
        [HttpPost]
        public IActionResult RemoveFriend(string userId)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!_userService.RemoveUserFromFriends(loginUserId, userId))
            {
                return View("~/Views/Shared/ErrorPage.cshtml");
            }
            return RedirectToAction("Friends");
        }

        [Authorize]
        [HttpGet]
        public IActionResult FriendRequests()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var friendsRequestsList = new List<List<FriendsList>>();
            friendsRequestsList.Add(_userService.GetFriendRequestsToUser(loginUserId));
            friendsRequestsList.Add(_userService.GetFriendRequestsFromUser(loginUserId));
            return View(friendsRequestsList);
        }

        [Authorize]
        [HttpPost]
        // accept or decline friend request to me
        public IActionResult FriendRequests(string acceptUserId, string declineUserId)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(acceptUserId != null)
            {
                if(_userService.AcceptFriendRequest(loginUserId, acceptUserId) == false) return View("~/Views/Shared/ErrorPage.cshtml");
                return RedirectToAction("Friends");
            }
            else
            {
                if (_userService.RemoveUserFromFriends(loginUserId, declineUserId) == false) return View("~/Views/Shared/ErrorPage.cshtml");
                return RedirectToAction("Friends");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CancelRequestFromUser(string userId)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User currentUser = await _userManager.FindByIdAsync(loginUserId);
            User friendUser = await _userManager.FindByIdAsync(userId);
            _userService.CancelFriendRequest(currentUser, friendUser);
            return RedirectToAction("Friends");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AddFriendRequest(string userId)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User mainUser = await _userManager.FindByIdAsync(loginUserId);
            User friendUser = await _userManager.FindByIdAsync(userId);
            if (_userService.SendFriendRequest(mainUser, friendUser) == false) return View("~/Views/Shared/ErrorPage.cshtml");
            return RedirectToAction("Friends");
        }



        [Authorize]
        [HttpGet]
        public IActionResult UsersList()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(_userService.GetUsersList(loginUserId));
        }


        [Authorize]
        [HttpPost]
        public IActionResult SearchUser(string searchText)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var users = _userService.GetUsersBySearch(searchText);
            return View(users);
        }
    }
}
