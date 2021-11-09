using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DataAccess.Context
{
    public class HobbyNetContext : IdentityDbContext<User>
    {
      


        public DbSet<Location> Locations { get; set; }
        public DbSet<Friends> FriendsList { get; set; }

        // public DbSet<Hobby> Hobbies { get; set; }
        // public DbSet<MainFeed> MainFeeds { get; set; }
        // public DbSet<Post> Posts { get; set; }
        // public DbSet<User> Users { get; set; }

        public HobbyNetContext(DbContextOptions<HobbyNetContext> options) : base(options)
        {
           //Database.EnsureDeleted();
           //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Friends>()
            .HasKey(f => new { f.MainUserId, f.FriendUserId });


            modelBuilder.Entity<Friends>()
                .HasOne(f => f.MainUser)
                .WithMany(mu => mu.MainUserFriends)
                .HasForeignKey(f => f.MainUserId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friends>()
                .HasOne(f => f.FriendUser)
                .WithMany(mu => mu.Friends)
                .HasForeignKey(f => f.FriendUserId).OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<User>().OwnsOne(v => v.Location);
        }

    }
}
