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

        public List<SubHobby> GetUserHobbiesList(string currentUserId)
        {
            var user = _context.Users.Where(u => u.Id == currentUserId).Include(u => u.SubHobbies).ThenInclude(s => s.Hobby).FirstOrDefault();
            var subHobbies = user.SubHobbies;
            return subHobbies;
        }

        public bool RemoveHobbyFromList(string currentUserId, int subHobbyId)
        {
            var user = _context.Users.Where(u => u.Id == currentUserId).Include(u => u.SubHobbies).ThenInclude(s => s.Hobby).FirstOrDefault();
            var subHobby = user.SubHobbies.Where(sh => sh.Id == subHobbyId).FirstOrDefault();
            user.SubHobbies.Remove(subHobby);
            _context.SaveChanges();
            return true;
        }
    }
}
