using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTO
{
    public class UsersList
    {
        public UsersList(string Email, string Id)
        {
            this.Email = Email;
            this.Id = Id;
        }

        public string Email { get; set; }
        public string Id { get; set; }
    }
}
