using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class HobbyViewModel
    {
        public List<AddHobbiesModel> addHobbiesList { get; set; }
        public List<Hobby> userHobbiesList { get; set; }   
    }

    public class AddHobbiesModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
