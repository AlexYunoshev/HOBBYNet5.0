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

        public List<Hobby> GetUserHobbies(string userId)
        {
            var hobbies = _context.Users.Where(u => u.Id == userId)
                .Include(u => u.Hobbies)
                .ThenInclude(h => h.ExplorePosts)
                .Select(u => u.Hobbies)
                .FirstOrDefault();
            return hobbies;
        }


        public List<List<ExplorePost>> GetRecommendedPostsList(string userId, out List<ExplorePost> posts, out Dictionary<Hobby, int> hobbyRatio2)
        {
            posts = GetExplorePosts();
            var likes = GetExploreLikes(userId);
            var comments = GetExploreComments(userId);
            var hobbyRatio = GetHobbyScores(likes, comments);
            hobbyRatio = GetHobbyRatio(hobbyRatio);
            var recommendedPosts = new List<List<ExplorePost>>();

            for (int i = 0; i < hobbyRatio.Count; i++)
            {
                recommendedPosts.Add(new List<ExplorePost>());
            }

            int index = 0;
            foreach (Hobby hobby in hobbyRatio.Keys)
            {
                recommendedPosts[index].AddRange(posts.Where(p => p.Hobbies.Contains(hobby)).ToList());
                posts.RemoveAll(p => p.Hobbies.Contains(hobby));
                recommendedPosts[index].RemoveAll(p => p.UserId == userId); // удаляем посты юзера (не рекомендовать ему же его посты).
                index++;
            }

            posts.RemoveAll(p => p.UserId == userId); // удаляем посты юзера (не рекомендовать ему же его посты).

            hobbyRatio2 = new Dictionary<Hobby, int>();
            recommendedPosts.Sort((a, b) => a.Count - b.Count);
            foreach (List<ExplorePost> explorePosts in recommendedPosts)
            {
                hobbyRatio2[explorePosts[0].Hobbies.FirstOrDefault()] = hobbyRatio[explorePosts[0].Hobbies.FirstOrDefault()];
            }

            return recommendedPosts;
        }


        static List<ExplorePost> GetPostsByPage(List<ExplorePost> recommendedPosts, int pageNumber)
        {
            var recommendedPostsOut = new List<ExplorePost>();
            int fullPagesCount = recommendedPosts.Count / 10;
            int startIndex = (pageNumber - 1) * 10;
            int count = 10;
            if (pageNumber > fullPagesCount)
            {
                startIndex = fullPagesCount * 10;
                count = recommendedPosts.Count - startIndex;
            }
            recommendedPostsOut.AddRange(recommendedPosts.GetRange(startIndex, count));
            return recommendedPostsOut;
        }


        static List<ExplorePost> GetPostsForRecommendations(List<ExplorePost> explorePostsWithoutRating,
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
                            recommendedPostsOut.Add(recommendedPosts[indexRecommendedHobbyPostsList][postIndex]);
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
            }

            while (explorePostsWithoutRating.Count != 0)
            {
                recommendedPostsOut.Add(explorePostsWithoutRating[0]);
                explorePostsWithoutRating.RemoveAt(0);
            }

            return recommendedPostsOut;
        }



        // отримуємо пріорітети хобі у балах. Наприклад: 24, 19, 17, 2
        static Dictionary<Hobby, int> GetHobbyScores(List<ExploreLike> likes, List<ExploreComment> comments)
        {
            var user = likes[0].User;
            var hobbyScores = new Dictionary<Hobby, int>();
            foreach (ExploreLike like in likes)
            {
                var hobbies = like.Post.Hobbies.ToList();

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

            foreach (ExploreComment comment in comments)
            {
                var hobbies = comment.Post.Hobbies.ToList();

                foreach (Hobby hobby in hobbies)
                {
                    if (hobbyScores.ContainsKey(hobby))
                    {
                        hobbyScores[hobby] += 4;
                    }
                    else
                    {
                        hobbyScores.Add(hobby, 4);
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
        static Dictionary<Hobby, int> GetHobbyRatio(Dictionary<Hobby, int> hobbyScores)
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

            return hobbyScores;
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
