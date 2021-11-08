using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTO
{
    public class UsersList
    {
        public UsersList(int Year, string Email, string Id)
        {
            this.Year = Year;
            this.Email = Email;
            this.Id = Id;
        }

        public int Year { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
    }
}
