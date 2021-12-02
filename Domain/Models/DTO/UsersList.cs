using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.DTO
{
    public class UsersList
    {
        public UsersList(string Username, string Id)
        {
            this.Username = Username;
            this.Id = Id;
        }

        public string Username { get; set; }
        public string Id { get; set; }
    }
}
