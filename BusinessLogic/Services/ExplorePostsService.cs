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

        public List<ExplorePost> GetExplorePostsByUser(string userId)
        {
            var posts = _context.ExplorePosts
                .Include(h => h.Hobbies)
                .Include(h => h.User)
                .Include(h => h.ExploreLikes).ThenInclude(l => l.User)
                .Include(h => h.ExploreComments).ThenInclude(c => c.User)
                .Where(p => p.UserId == userId)
                .ToList();
            return posts;
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

        public ExplorePost GetExplorePost(int postId)
        {
            var post = _context.ExplorePosts
                .Include(h => h.Hobbies)
                .Include(h => h.User)
                .Include(h => h.ExploreLikes).ThenInclude(l => l.User)
                .Include(h => h.ExploreComments).ThenInclude(c => c.User)
                .Where(p => p.Id == postId)
                .FirstOrDefault();
            return post;
        }

        // отримуємо пріорітети хобі у балах. Наприклад: 24, 19, 17, 2
        public Dictionary<Hobby, int> GetHobbyScores(List<ExploreLike> likes, List<ExploreComment> comments, string userId)
        {
            var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
            var hobbyScores = new Dictionary<Hobby, int>();

            foreach (ExploreLike like in likes)
            {
                var hobbies = like.Post.Hobbies.ToList();

                foreach (Hobby hobby in hobbies)
                {
                    if (hobbyScores.ContainsKey(hobby))
                    {
                        hobbyScores[hobby] += 1;
                    }
                    else
                    {
                        hobbyScores.Add(hobby, 1);
                    }
                }
            }

            foreach (ExploreComment comment in comments)
            {
                var hobbies = comment.Post.Hobbies.ToList();

                foreach (Hobby hobby in hobbies)
                {
                    if (hobbyScores.ContainsKey(hobby))
                    {
                        hobbyScores[hobby] += 2;
                    }
                    else
                    {
                        hobbyScores.Add(hobby, 2);
                    }
                }
            }

            foreach (Hobby hobby in hobbyScores.Keys)
            {

                if (user.Hobbies.Contains(hobby))
                {
                    hobbyScores[hobby] += 15;
                }
            }
            return hobbyScores;
        }

        // отримуємо пріорітети хобі у відношенні до 10. Наприклад: 4, 3, 3, 0.
        // Якщо маємо 0, то таке хобі видаляється.
        public Dictionary<Hobby, int> GetHobbyRatio(Dictionary<Hobby, int> hobbyScores)
        {
            int sumScore = 0;
            foreach (int value in hobbyScores.Values)
            {
                sumScore += value;
            }

            foreach (Hobby hobby in hobbyScores.Keys)
            {
                double value = ((double)hobbyScores[hobby] / (double)sumScore) * 10;
                int hobbyScore = (int)Math.Round(value, 0, MidpointRounding.ToEven);
                if (hobbyScore != 0) { hobbyScores[hobby] = hobbyScore; }
                else { hobbyScores.Remove(hobby); }
            }
            if (hobbyScores.Values.Sum() > 10)
            {
                var keyByMinValue = hobbyScores.Aggregate((a, b) => a.Value < b.Value ? a : b).Key;
                //var keyByMinValue = hobbyScores.OrderBy(v => v.Value).FirstOrDefault().Key;
                hobbyScores[keyByMinValue] -= (hobbyScores.Values.Sum() - 10);
                if (hobbyScores[keyByMinValue] == 0) { hobbyScores.Remove(keyByMinValue); }
            }

            return hobbyScores;
        }

        public List<List<ExplorePost>> GetRecommendedPostsList
            (string userId, out List<ExplorePost> posts, out Dictionary<Hobby, int> hobbyRatioOut)
        {
            posts = GetExplorePosts();
            var likes = GetExploreLikes(userId);
            var comments = GetExploreComments(userId);
            var hobbyRatio = GetHobbyScores(likes, comments, userId);
            hobbyRatio = GetHobbyRatio(hobbyRatio);
            var recommendedPosts = new List<List<ExplorePost>>();

            // видаляємо пости користувача, щоб не рекомендувати йому його ж пости (з іншими хобі)
            posts.RemoveAll(p => p.UserId == userId);

            // видаляємо пости, які вже лайкнув юзер
            posts.RemoveAll(p => p.ExploreLikes.Any(l => l.UserId == userId));

            for (int i = 0; i < hobbyRatio.Count; i++)
            {
                recommendedPosts.Add(new List<ExplorePost>());
            }

            int index = 0;
            foreach (Hobby hobby in hobbyRatio.Keys)
            {
                recommendedPosts[index].AddRange(posts.Where(p => p.Hobbies.Contains(hobby)).ToList());
                posts.RemoveAll(p => p.Hobbies.Contains(hobby));
                index++;
            }

            hobbyRatioOut = new Dictionary<Hobby, int>();
            recommendedPosts.Sort((a, b) => a.Count - b.Count);
            foreach (List<ExplorePost> explorePosts in recommendedPosts)
            {
                hobbyRatioOut[explorePosts[0].Hobbies.FirstOrDefault()] = hobbyRatio[explorePosts[0].Hobbies.FirstOrDefault()];
            }

            return recommendedPosts;
        }

        public List<ExplorePost> GetPostsForRecommendations(List<ExplorePost> explorePostsWithoutRating,
            Dictionary<Hobby, int> postRatingByHobbies, List<List<ExplorePost>> recommendedPosts)
        {
            var recommendedPostsOut = new List<ExplorePost>();

            int postOnPageCount;
            int postIndex;
            int allRecommendedPostsCount = 0;
            int indexRecommendedHobbyPostsList = 0;

            foreach (int postCount in postRatingByHobbies.Values)
            {
                allRecommendedPostsCount += postCount;
            }

            List<ExplorePost> recommendedPostsPage = new List<ExplorePost>();

            // робимо прохід по 10 сторінкам. Можна ще зробити while recommendedPosts.count != 0
            for (int counter = 0; counter < 10; counter++)
            {
                indexRecommendedHobbyPostsList = 0;
                foreach (Hobby hobby in postRatingByHobbies.Keys)
                {
                    postOnPageCount = postRatingByHobbies[hobby];
                    if (postRatingByHobbies.Count == 1)
                    {
                        postOnPageCount = recommendedPosts[indexRecommendedHobbyPostsList].Count;
                    }

                    for (int i = 0; i < postOnPageCount; i++)
                    {
                        postIndex = 0;
                        if (postIndex < recommendedPosts[indexRecommendedHobbyPostsList].Count)
                        {
                            recommendedPostsPage.Add(recommendedPosts[indexRecommendedHobbyPostsList][postIndex]);
                            recommendedPosts[indexRecommendedHobbyPostsList].RemoveAt(postIndex);
                        }

                        if (postIndex == recommendedPosts[indexRecommendedHobbyPostsList].Count)
                        {
                            postRatingByHobbies.Remove(hobby);
                            recommendedPosts.RemoveAt(indexRecommendedHobbyPostsList);
                            indexRecommendedHobbyPostsList--;
                            break;
                        }
                    }
                    indexRecommendedHobbyPostsList++;
                }
                recommendedPostsPage.Reverse();
                recommendedPostsOut.AddRange(recommendedPostsPage);
                recommendedPostsPage.Clear();
            }

            while (explorePostsWithoutRating.Count != 0)
            {
                recommendedPostsOut.Add(explorePostsWithoutRating[0]);
                explorePostsWithoutRating.RemoveAt(0);
            }

            return recommendedPostsOut;
        }


        public List<ExplorePost> GetPostsByPage(List<ExplorePost> posts, int pageNumber)
        {
            var postsOut = new List<ExplorePost>();
            int fullPagesCount = posts.Count / 10;
            int startIndex = (pageNumber - 1) * 10;
            int count = 10;
            if (pageNumber > fullPagesCount)
            {
                startIndex = fullPagesCount * 10;
                count = posts.Count - startIndex;
            }
            postsOut.AddRange(posts.GetRange(startIndex, count));
            return postsOut;
        }

        public void AddCommentToPost(int postId, string userId, string commentText)
        {
            var post = GetExplorePost(postId);
            post.ExploreComments.Add(new ExploreComment() { PostId = postId, UserId = userId, Text = commentText });
            _context.SaveChanges();
        }

        public void RemoveCommentFromPost(int postId, int commentId)
        {
            var post = GetExplorePost(postId);
            var comment = post.ExploreComments.Where(c => c.Id == commentId).FirstOrDefault();
            if (comment != null)
            {
                _context.ExploreComments.Remove(comment);
                _context.SaveChanges();
            }
        }

        public void SetLikeToPost(int postId, string userId)
        {
            var post = GetExplorePost(postId);
            var like = post.ExploreLikes.Where(l => l.UserId == userId).FirstOrDefault();
            if (like != null)
            {
                post.ExploreLikes.Remove(like);
            }
            else
            {
                post.ExploreLikes.Add(new ExploreLike() { PostId = postId, UserId = userId });
            }
            _context.SaveChanges();
        }
    }
}
