using DataAccess.Context;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ExplorePostsService
    {
        private readonly HobbyNetContext _context;

        public ExplorePostsService(HobbyNetContext context)
        {
            _context = context;
        }

        public List<ExploreLike> GetExploreLikes(string currentUserId)
        {
            var likes = _context.ExploreLikes
                .Where(u => u.UserId == currentUserId)
                .Include(p => p.Post)
                .ThenInclude(h => h.Hobbies)
                .ToList();
            return likes;
        }

        public List<ExploreComment> GetExploreComments(string currentUserId)
        {
            var comments = _context.ExploreComments
                .Where(u => u.UserId == currentUserId)
                .Include(p => p.Post)
                .ThenInclude(h => h.Hobbies)
                .ToList();
            return comments;
        }

        public List<ExplorePost> GetExplorePosts()
        {
            var posts = _context.ExplorePosts
                .Include(h => h.Hobbies)
                .Include(h => h.User)
                .Include(h => h.ExploreLikes).ThenInclude(l => l.User)
                .Include(h => h.ExploreComments).ThenInclude(c => c.User)
                .ToList();
            return posts;
        }

       
    }
}
