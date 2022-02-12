using System;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DecaBlog.Models;
using Microsoft.EntityFrameworkCore;

namespace DecaBlog.Data
{
    public class DecaBlogDbContext : IdentityDbContext<User>
    {
        public DecaBlogDbContext(DbContextOptions<DecaBlogDbContext> options)
            : base(options) { }

        public DbSet<Photo> Photos { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Stack> Stacks { get; set; }
        public DbSet<Squad> Squads { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleTopic> ArticleTopics { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<UserSquad> UserSquad { get; set; }
        public DbSet<UserStack> UserStack { get; set; }
        public DbSet<Invitee> Invitees { get; set; }

        public DbSet<UserComment> UserComments { get; set; }
        public DbSet<Notification> Notifications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserSquad>()
                .HasKey(c => new { c.Id, c.UserId });
            modelBuilder.Entity<UserStack>()
                .HasKey(c => new { c.Id, c.UserId });
            modelBuilder.Entity<UserComment>()
                .HasKey(c => new { c.Id, c.CommenterId, c.TopicId });
            
            base.OnModelCreating(modelBuilder);
        }
       

    }
}
