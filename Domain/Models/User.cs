﻿using Domain.Interfaces;
using System.Collections.Generic;

namespace Domain.Models
{
    public class User : IEntity<int>
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PhotoPath { get; set; }
        public Location Location { get; set; }
        public Account Account { get; set; }
        public List<User> FriendsList { get; set; }
    }
}