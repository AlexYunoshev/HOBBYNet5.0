using DataAccess.Context;
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
    }
}
