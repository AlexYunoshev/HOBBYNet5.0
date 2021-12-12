using BusinessLogic.Services;
using Domain.Models;
using Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HOBBYNetMVC.Controllers
{
    public class ExplorePostsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ExplorePostsService _explorePostsService;
        private readonly UserService _userService;

        public ExplorePostsController(UserManager<User> userManager, SignInManager<User> signInManager, ExplorePostsService explorePostsService, UserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _explorePostsService = explorePostsService;
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index(int locationRadius = 0, string sort = "newest", int pageNumber = 1)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User currentUser = _userService.GetUserById(loginUserId);
            var posts = _explorePostsService.GetExplorePosts();
            posts.RemoveAll(p => p.User == currentUser);
            var model = new ExplorePostsViewModel(posts.Count, currentUser, posts, pageNumber, locationRadius, sort, false)
            {
                CurrentPageNumber = pageNumber,
                AuthorizedUserId = loginUserId
            };
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult RecommendedPosts(int locationRadius = 0, string sort = "newest", int pageNumber = 1)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User currentUser = _userService.GetUserById(loginUserId);
            List<ExplorePost> explorePostsWithoutRating;
            Dictionary<Hobby, int> postRatingByHobbies;
            var recommendedPosts = _explorePostsService.GetRecommendedPostsList(loginUserId, out explorePostsWithoutRating, out postRatingByHobbies);
            var recommendedPostsList = _explorePostsService.GetPostsForRecommendations(explorePostsWithoutRating, postRatingByHobbies, recommendedPosts);
            var model = new ExplorePostsViewModel(recommendedPostsList.Count, currentUser, recommendedPostsList, 
                pageNumber, locationRadius, sort, true)
            {
                CurrentPageNumber = pageNumber,
                AuthorizedUserId = loginUserId,
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult SetLikeToPost(int postId, int pageNumber)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _explorePostsService.SetLikeToPost(postId, loginUserId);
            string url = "Index?pageNumber=" + pageNumber + "#post-" + postId;
            return Redirect(url);
        }

        [Authorize]
        [HttpPost]
        public IActionResult SetLikeToRecommendedPost(int postId, int pageNumber)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _explorePostsService.SetLikeToPost(postId, loginUserId);
            string url = "RecommendedPosts?pageNumber=" + pageNumber + "#post-" + postId;
            return Redirect(url);
        }

        [Authorize]
        [HttpGet]
        public IActionResult SetCommentToPost(int postId)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = _explorePostsService.GetExplorePost(postId);
            return View("PostComments", post);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddCommentToPost(string commentText, int postId, int pageNumber)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _explorePostsService.AddCommentToPost(postId, loginUserId, commentText);
            var post = _explorePostsService.GetExplorePost(postId);
            string url = "Index?pageNumber=" + pageNumber + "#post-" + postId;
            return Redirect(url);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddCommentToRecommendedPost(string commentText, int postId, int pageNumber)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _explorePostsService.AddCommentToPost(postId, loginUserId, commentText);
            var post = _explorePostsService.GetExplorePost(postId);
            string url = "RecommendedPosts?pageNumber=" + pageNumber + "#post-" + postId;
            return Redirect(url);
        }

        [Authorize]
        [HttpPost]
        public IActionResult RemoveCommentFromPost(int commentId, int postId, int pageNumber)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = _explorePostsService.GetExplorePost(postId);
            _explorePostsService.RemoveCommentFromPost(postId, commentId);
            string url = "Index?pageNumber=" + pageNumber + "#post-" + postId;
            return Redirect(url);
        }

        [Authorize]
        [HttpPost]
        public IActionResult RemoveCommentFromRecommendedPost(int commentId, int postId, int pageNumber)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = _explorePostsService.GetExplorePost(postId);
            _explorePostsService.RemoveCommentFromPost(postId, commentId);
            string url = "RecommendedPosts?pageNumber=" + pageNumber + "#post-" + postId;
            return Redirect(url);
        }
    }
}
