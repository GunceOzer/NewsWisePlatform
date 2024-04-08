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
}