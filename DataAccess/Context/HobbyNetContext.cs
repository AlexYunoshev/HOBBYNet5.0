using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace DataAccess.Context
{
    public class HobbyNetContext : DbContext
    {
        public DbSet<Hobby> Hobbies { get; set; }
        public DbSet<MainFeed> MainFeeds { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
