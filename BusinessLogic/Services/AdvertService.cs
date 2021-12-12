using DataAccess.Context;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class AdvertService
    {
        private readonly HobbyNetContext _context;

        public AdvertService(HobbyNetContext context)
        {
            _context = context;
        }

        public List<Advert> GetAllAdverts()
        {
            return _context.Adverts
                .Include(a => a.Hobbies)
                .Include(a => a.User).ThenInclude(u => u.Location)
                .ToList();
        }

        public List<Advert> GetUserAdverts(string userId)
        {
            return _context.Adverts
                .Include(a => a.Hobbies)
                .Include(a => a.User).ThenInclude(u => u.Location)
                .Where(a => a.UserId == userId)
                .ToList();
        }

        public Advert GetAdvert(int advertId)
        {
            return _context.Adverts
                .Include(a => a.Hobbies)
                .Include(a => a.User)
                .Where(a => a.Id == advertId)
                .FirstOrDefault();
        }

        public void RemoveAdvert(string userId, int advertId)
        {
            var advert = GetAdvert(advertId);
            if (advert.UserId == userId)
            {
                _context.Adverts.Remove(advert);
                _context.SaveChanges();
            }  
        }

        public void AddAdvert(User user, string messageText, List<Hobby> hobbies)
        {
            var advert = new Advert() { User = user, Message = messageText};

            _context.Adverts.Add(advert);
            _context.SaveChanges();

            var addedAdvert = _context.Adverts
                .Where(a => a.UserId == user.Id)
                .OrderByDescending(p => p.Id)
                .FirstOrDefault();
            foreach (var hobby in hobbies)
            {
                addedAdvert.Hobbies.Add(hobby);
            }

            _context.SaveChanges();
        }

    }
}
