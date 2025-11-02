using BlogWebApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApplication.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        public DbSet<Category> Categories { get; set; } //this line will create table for this model

        public DbSet<Post> Posts { get; set; } //this line will create table for this model

        public DbSet<Comment> Comments { get; set; } //this line will create table for this model

        //seeding initial data through overriding OnModelCreating method
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); //calling the base method
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Technology", Description = "Posts related to technology." },
                new Category { Id = 2, Name = "Health", Description = "Posts related to health and wellness." },
                new Category { Id = 3, Name = "Travel", Description = "Posts about travel experiences and tips." }
            );
            modelBuilder.Entity<Post>().HasData(
                new Post
                {
                    Id = 1,
                    Title = "The Rise of AI in Everyday Life",
                    Author = "John Doe",
                    Content = "Artificial Intelligence (AI) is becoming increasingly prevalent in our daily lives...",
                    PublishedDate = new DateTime(2024, 3, 1),
                    CategoryId = 1
                },
                new Post
                {
                    Id = 2,
                    Title = "10 Tips for a Healthier Lifestyle",
                    Author = "Jane Smith",
                    Content = "Living a healthier lifestyle doesn't have to be complicated. Here are 10 simple tips...",
                    PublishedDate = new DateTime(2024, 3, 1),
                    CategoryId = 2
                },
                new Post
                {
                    Id = 3,
                    Title = "Top 5 Travel Destinations for 2024",
                    Author = "Emily Johnson",
                    Content = "Looking for your next adventure? Here are the top 5 travel destinations to consider in 2024...",
                    PublishedDate = new DateTime(2024, 3, 1),
                    CategoryId = 3
                }
            );
        }

    }
}
