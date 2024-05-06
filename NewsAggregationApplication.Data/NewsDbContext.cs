using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsAggregationApplication.Data.Entities;

namespace NewsAggregationApplication.Data;

public class NewsDbContext:IdentityDbContext<User,IdentityRole<Guid>,Guid>
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<Source> Sources { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }
    
    public NewsDbContext(DbContextOptions<NewsDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        
        base.OnModelCreating(builder);
        // when article is deleted comments are also deleted
        builder.Entity<Article>()
            .HasMany(c => c.Comments)
            .WithOne(e => e.Article)
            .HasForeignKey(e => e.ArticleId)
            .OnDelete(DeleteBehavior.Cascade); 

        //when article is deleted likes are also deleted
        builder.Entity<Article>()
            .HasMany(l => l.Likes)
            .WithOne(e => e.Article)
            .HasForeignKey(e => e.ArticleId)
            .OnDelete(DeleteBehavior.Cascade); 

        //when article is deleted bookmarks are also deleted 
        builder.Entity<Article>()
            .HasMany(b => b.Bookmarks)
            .WithOne(e => e.Article)
            .HasForeignKey(e => e.ArticleId)
            .OnDelete(DeleteBehavior.Cascade); 
        
        // when the user is deleted comments are also deleted
        builder.Entity<User>()
            .HasMany(u => u.Comments)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // when user is deleted likes are also be deleted
        builder.Entity<User>()
            .HasMany(u => u.Likes)
            .WithOne(l => l.User)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // When the user is deleted bookmarks are also be deleted
        builder.Entity<User>()
            .HasMany(u => u.Bookmarks)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}