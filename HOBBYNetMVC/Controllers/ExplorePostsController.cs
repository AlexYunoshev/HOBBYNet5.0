using BusinessLogic.Services;
using Domain.Models;
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

        public ExplorePostsController(UserManager<User> userManager, SignInManager<User> signInManager, ExplorePostsService explorePostsService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _explorePostsService = explorePostsService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            var posts = _explorePostsService.GetExplorePosts();
            return View(posts);
        }

        [Authorize]
        [HttpGet]
        public IActionResult RecommendedPosts()
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
          
            List<ExplorePost> explorePostsWithoutRating;
            Dictionary<Hobby, int> postRatingByHobbies;
            var recommendedPosts = _explorePostsService.GetRecommendedPostsList(loginUserId, out explorePostsWithoutRating, out postRatingByHobbies);
            var recommendedPostsList = _explorePostsService.GetPostsForRecommendations(explorePostsWithoutRating, postRatingByHobbies, recommendedPosts);
            return View(recommendedPostsList);
        }

        [Authorize]
        [HttpPost]
        public IActionResult SetLikeToPost(int postId)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _explorePostsService.SetLikeToPost(postId, loginUserId);
           
            return RedirectToAction("Index");
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
        public IActionResult AddCommentToPost(string commentText, int postId)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _explorePostsService.AddCommentToPost(postId, loginUserId, commentText);
            var post = _explorePostsService.GetExplorePost(postId);
            return View("PostComments", post);
        }

        [Authorize]
        [HttpPost]
        public IActionResult RemoveCommentFromPost(int commentId, int postId)
        {
            var loginUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = _explorePostsService.GetExplorePost(postId);
            _explorePostsService.RemoveCommentFromPost(postId, commentId);
            return View("PostComments", post);
        }



    }
}
