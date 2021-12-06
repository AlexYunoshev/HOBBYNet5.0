using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class EditPostViewModel
    {
        public HobbyViewModel HobbyViewModel { get; set; }
        public ExplorePost Post { get; set; }
    }
}
