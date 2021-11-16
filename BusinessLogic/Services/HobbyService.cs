﻿using DataAccess.Context;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class HobbyService
    {
        private readonly HobbyNetContext _context;

        public HobbyService(HobbyNetContext context)
        {
            _context = context;
        }

        public List<Hobby> GetUserHobbiesList(string currentUserId)
        {
            var user = _context.Users.Where(u => u.Id == currentUserId).Include(u => u.Hobbies).FirstOrDefault();
            var hobbies = user.Hobbies;
            return hobbies;
        }

        public bool RemoveHobbyFromList(string currentUserId, int hobbyId)
        {
            var user = _context.Users.Where(u => u.Id == currentUserId).Include(u => u.Hobbies).FirstOrDefault();
            var hobby = user.Hobbies.Where(h => h.Id == hobbyId).FirstOrDefault();
            user.Hobbies.Remove(hobby);
            _context.SaveChanges();
            return true;
        }
    }
}
