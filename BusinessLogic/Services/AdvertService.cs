using DataAccess.Context;
using Domain.Models;
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
    }
}
