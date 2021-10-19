using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Interfaces;

namespace DataAccess.Context
{
    public class HobbyNetContext : DbContext
    {
        public DbSet<Hobby> Hobbies { get; set; }
        //public DbSet<Location> Locations { get; set; }
        //public DbSet<MainFeed> MainFeeds { get; set; }
        //public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        
        public HobbyNetContext(DbContextOptions<HobbyNetContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           //modelBuilder.Entity<User>().OwnsOne(v => v.Location);
        }

    }
}
