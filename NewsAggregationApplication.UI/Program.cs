using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.CommandHandlers.Article;
using NewsAggregationApplication.UI.CQS.CommandHandlers.Comment;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Mappers;
using NewsAggregationApplication.UI.Services;


namespace NewsAggregationApplication.UI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        
        builder.Services.AddControllersWithViews();
        
        builder.Services.AddDbContext<NewsDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("NewsAggregationDatabase")));
        builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                // For secure password
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole<Guid>>() 
            .AddEntityFrameworkStores<NewsDbContext>()
            .AddDefaultTokenProviders();

        
        builder.Services.AddHttpClient();
        builder.Services.AddScoped<IArticleService, ArticleService>();
        builder.Services.AddScoped<ILikeService,LikeService>();
        builder.Services.AddScoped<IBookmarkService, BookmarkService>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IImageExtractor, ImageExtractor>();
        builder.Services.AddScoped<IContentScraper, ContentScraper>();
        builder.Services.AddScoped<IJwtTokenService,JwtTokenService>();
        builder.Services.AddHttpClient<IEmailService, EmailService>();
        builder.Services.AddScoped<IInactiveUserNotificationService,InactiveUserNotificationService>();
        builder.Services.AddHostedService<StartupHostedService>();
        builder.Services.AddScoped<ISentimentAnalysisService,SentimentAnalysisService>();
       


        builder.Services.AddScoped<ArticleMapper>();
        builder.Services.AddScoped<LikeMapper>();
        builder.Services.AddScoped<BookmarkMapper>();
        builder.Services.AddScoped<CommentMapper>();
        
        
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(
                typeof(AddCommentCommandHandler).Assembly));
        
        //Hangfire service
        builder.Services.AddHangfire(conf => conf
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseDefaultTypeSerializer()
            .UseSqlServerStorage(builder.Configuration.GetConnectionString("NewsAggregationDatabase")));

        builder.Services.AddHangfireServer();

        
        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
            await accountService.CreateRoles(scope.ServiceProvider);
        }
       

      
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHangfireDashboard();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();


        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Article}/{action=Index}/{id?}");
        
        app.Run();
    }
    
}


