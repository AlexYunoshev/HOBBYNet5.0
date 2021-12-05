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


        public UserController(
            UserManager<User> userManager, SignInManager<User> signInManager, 
            UserService userService, ExplorePostsService explorePostsService, 
            ILogger<UserController> logger, IWebHostEnvironment appEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _explorePostsService = explorePostsService;
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
            User currentUser = _userManager.FindByIdAsync(loginUserId).Result;

            return View(new ExplorePostsViewModel(posts.Count, currentUser, posts, pageNumber, 0) { CurrentPageNumber = pageNumber });
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddPost()
        {
            //var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
          
            //User currentUser = _userManager.FindByIdAsync(loginUserId).Result;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPost(string postText, IFormFile file)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User currentUser = _userManager.FindByIdAsync(loginUserId).Result;
            var path = _appEnvironment.WebRootPath;
            var filePath = Path.Combine(path, "images");
            filePath = Path.Combine(filePath, currentUser.UserName);
            if (file != null)
            {
                if (file.FileName.ToUpper().EndsWith(".JPG") || file.FileName.ToUpper().EndsWith(".PNG"))
                {
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    //filePath = Path.Combine(filePath, file.FileName);

                    filePath = Path.Combine(filePath, "post-12313");
                    if (file.FileName.ToUpper().EndsWith(".JPG"))
                    {
                        filePath += ".jpg";
                    }
                    else if (file.FileName.ToUpper().EndsWith(".PNG"))
                    {
                        filePath += ".png";
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    _explorePostsService.AddPost(loginUserId, postText, filePath);
                }
            }

            

            else
            {
                _explorePostsService.AddPost(loginUserId, postText);
            }

           
            return RedirectToAction("Profile");
        }


        [Authorize]
        [HttpPost]
        public IActionResult SetLikeToPost(int postId, int pageNumber)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _explorePostsService.SetLikeToPost(postId, loginUserId);
            string url = "Profile?pageNumber="+pageNumber+"#post-" + postId;
            return Redirect(url);
        }


        [Authorize]
        [HttpPost]
        public IActionResult AddCommentToPost(string commentText, int postId, int pageNumber)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _explorePostsService.AddCommentToPost(postId, loginUserId, commentText);
            var post = _explorePostsService.GetExplorePost(postId);
            string url = "Profile?pageNumber=" + pageNumber + "#post-" + postId;
            return Redirect(url);
            //return RedirectToAction("Profile");
            //return View("PostComments", post);
        }

        [Authorize]
        [HttpPost]
        public IActionResult RemoveCommentFromPost(int commentId, int postId, int pageNumber)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = _explorePostsService.GetExplorePost(postId);
            _explorePostsService.RemoveCommentFromPost(postId, commentId);
            string url = "Profile?pageNumber=" + pageNumber + "#post-" + postId;
            return Redirect(url);
            //return View("PostComments", post);
        }







        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        [HttpGet]
        public IActionResult UsersList()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(_userService.GetUsersList(loginUserId));
        }

        [Authorize]
        [HttpGet]
        public IActionResult Friends()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(_userService.GetFriendsList(loginUserId));
        }

        [Authorize]
        [HttpPost]
        public IActionResult Friends(string userId)
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
                return RedirectToAction("Friends", "User");
            }
            else
            {
                if (_userService.RemoveUserFromFriends(loginUserId, declineUserId) == false) return View("~/Views/Shared/ErrorPage.cshtml");
                return RedirectToAction("FriendRequests", "User");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AddFriendRequest(string id)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User mainUser = await _userManager.FindByIdAsync(loginUserId);
            User friendUser = await _userManager.FindByIdAsync(id);
            if (_userService.SendFriendRequest(mainUser, friendUser) == false) return View("~/Views/Shared/ErrorPage.cshtml");
            return RedirectToAction("FriendRequests");
        }
    }
}
