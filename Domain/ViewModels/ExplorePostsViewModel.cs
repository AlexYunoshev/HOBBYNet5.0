using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class ExplorePostsViewModel
    {
        public List<ExplorePost> Posts { get; set; }
        public int AllPostsCount { get; set; }
        public int PagesCount { get; set; }
        public int CurrentPageNumber { get; set; }


        
        public ExplorePostsViewModel(int allPostsCount)
        {
            AllPostsCount = allPostsCount;
            if (AllPostsCount % 10 == 0)
            {
                PagesCount = AllPostsCount / 10;
            }
            else
            {
                PagesCount = AllPostsCount / 10 + 1;
            }
        }
    }
}
