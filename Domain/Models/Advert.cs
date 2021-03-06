using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Advert : IEntity<int>
    {
        public int Id { get; set; }
        public string PhotoPath { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public List<Hobby> Hobbies { get; set; } = new List<Hobby>();
        public string Message { get; set; }
       

        [ForeignKey("UserId")]
        public User User { get; set; }

        //public Advert(List<Hobby> hobbies)
        //{
        //    Hobbies = hobbies;
        //}
    }

}
