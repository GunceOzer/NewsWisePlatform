using Microsoft.EntityFrameworkCore;
namespace NewsWisePlatform.Models;

public class NewsWiseDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<NewsItem> NewsItems { get; set; }
    public DbSet<NewsSource> NewsSources { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<UserPreference> UserPreferences { get; set; }
    public DbSet<CommentFlag> CommentFlags { get; set; }
    public DbSet<MediaAttachment> MediaAttachments { get; set; }
    public DbSet<ExternalLogin> ExternalLogins { get; set; }
    public DbSet<NewsItemBookmark> NewsItemBookmarks { get; set; }
    public DbSet<NewsItemShare> NewsItemShares { get; set; }
    public DbSet<NewsItemLike> NewsItemLikes { get; set; }
    public DbSet<NewsItemTopic> NewsItemTopics { get; set; }
    public DbSet<EmailNotification> EmailNotifications { get; set; }


    public NewsWiseDbContext(DbContextOptions<NewsWiseDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

       

        // User to Admin relationship (one to one)
        modelBuilder.Entity<Admin>()
            .HasOne(a => a.User)
            .WithOne()
            .HasForeignKey<Admin>(a => a.UserID);

        // User to Comments (one-to-many)
        modelBuilder.Entity<User>()
            .HasMany(u => u.Comments)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserID);
        
        //if an account deleted the likes are also be deleted
        modelBuilder.Entity<User>()
            .HasMany(ni => ni.Likes)
            .WithOne(l => l.User)
            .HasForeignKey(c => c.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        // User to External Login (one-to-one)
        modelBuilder.Entity<User>()
            .HasOne(u => u.ExternalLogin)
            .WithOne(el => el.User)
            .HasForeignKey<ExternalLogin>(el => el.UserID);

        // NewsItem to Comments (one-to-many)
        modelBuilder.Entity<NewsItem>()
            .HasMany(ni => ni.Comments)
            .WithOne(c => c.NewsItem)
            .HasForeignKey(c => c.NewsItemID)
            .OnDelete(DeleteBehavior.Cascade); //if the news deleted, comments are also be deleted

        // NewsItem to MediaAttachments (one-to-many)
        modelBuilder.Entity<NewsItem>()
            .HasMany(ni => ni.MediaAttachments)
            .WithOne(ma => ma.NewsItem)
            .HasForeignKey(ma => ma.NewsItemID);
        

        // NewsItem to NewsItemLikes (one-to-many)
        modelBuilder.Entity<NewsItem>()
            .HasMany(ni => ni.Likes)
            .WithOne(l => l.NewsItem)
            .HasForeignKey(l => l.NewsItemID);

        // NewsItem to NewsItemBookmarks (one-to-many)
        modelBuilder.Entity<NewsItem>()
            .HasMany(ni => ni.Bookmarks)
            .WithOne(b => b.NewsItem)
            .HasForeignKey(b => b.NewsItemID);

        // NewsItem to NewsItemShares (one-to-many)
        modelBuilder.Entity<NewsItem>()
            .HasMany(ni => ni.Shares)
            .WithOne(s => s.NewsItem)
            .HasForeignKey(s => s.NewsItemID);

        // Many-to-Many: NewsItem to Topic through NewsItemTopic
        modelBuilder.Entity<NewsItem>()
            .HasMany(ni => ni.Topics)
            .WithMany(t => t.NewsItems)
            .UsingEntity<NewsItemTopic>(
                j => j.HasOne(nit => nit.Topic).WithMany(t => t.NewsItemTopics).HasForeignKey(nit => nit.TopicID),
                j => j.HasOne(nit => nit.NewsItem).WithMany(ni => ni.NewsItemTopics)
                    .HasForeignKey(nit => nit.NewsItemID),
                j => j.HasKey(nit => new { nit.NewsItemID, nit.TopicID })
            );


        modelBuilder.Entity<NewsItemShare>()
            .HasKey(s => s.ShareID); // Primary key

        modelBuilder.Entity<NewsItemShare>() //not sure if it is one to many
            .HasOne(s => s.User) // Share has one User
            .WithMany(u => u.NewsItemShares) // User has many Shares
            .HasForeignKey(s => s.UserID); // Foreign key on Share

        modelBuilder.Entity<NewsItemShare>()
            .HasOne(s => s.NewsItem) // Share has one NewsItem
            .WithMany(ni => ni.Shares) // NewsItem has many Shares
            .HasForeignKey(s => s.NewsItemID);
        
        modelBuilder.Entity<UserPreference>()
            .HasKey(up => up.PreferenceID);

    

        modelBuilder.Entity<User>().HasKey(u => u.UserID);
        modelBuilder.Entity<NewsSource>().HasKey(ns => ns.SourceID);
        modelBuilder.Entity<NewsItem>().HasKey(ni => ni.NewsItemID);
        modelBuilder.Entity<UserPreference>().HasKey(up => up.PreferenceID);
        modelBuilder.Entity<EmailNotification>().HasKey(en => en.NotificationID);
        modelBuilder.Entity<NewsItemBookmark>().HasKey(b => b.BookmarkID); 
        
       
    }
}          