using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Services;

namespace NewsAggregationApplication.UI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
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
            .AddEntityFrameworkStores<NewsDbContext>();
        
       

        builder.Services.AddScoped<IArticleService, ArticleService>();
        builder.Services.AddScoped<ILikeService,LikeService>();
        builder.Services.AddScoped<IBookmarkService, BookmarkService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

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


