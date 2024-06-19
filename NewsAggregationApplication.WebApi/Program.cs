using System.Text;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NewsAggregationApplication.Data;
using NewsAggregationApplication.Data.Entities;
using NewsAggregationApplication.UI.CQS.CommandHandlers.Comment;
using NewsAggregationApplication.UI.Interfaces;
using NewsAggregationApplication.UI.Mappers;
using NewsAggregationApplication.UI.Services;

namespace NewsAggregationApplication.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        const string myCorsPolicy = "AllowEverything";
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
       // builder.Services.AddSwaggerGen();
       builder.Services.AddSwaggerGen(c =>
       {
           c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    
           // Add JWT Authentication
           c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
           {
               Name = "Authorization",
               Type = SecuritySchemeType.ApiKey,
               Scheme = "Bearer",
               BearerFormat = "JWT",
               In = ParameterLocation.Header,
               Description = "JWT Authorization header using the Bearer scheme."
           });
    
           c.AddSecurityRequirement(new OpenApiSecurityRequirement
           {
               {
                   new OpenApiSecurityScheme
                   {
                       Reference = new OpenApiReference
                       {
                           Type = ReferenceType.SecurityScheme,
                           Id = "Bearer"
                       }
                   },
                   new string[] {}
               }
           });
       });
       
       builder.Logging.ClearProviders();
       builder.Logging.AddConsole();
       
        builder.Services.AddAuthorization();

        builder.Services.AddDbContext<NewsDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("NewsAggregationDatabase")));

        builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 6;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<NewsDbContext>().AddDefaultTokenProviders();//addtoken is added later it is a trial 
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

        builder.Services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero 
                };
            });

        

        builder.Services.AddHttpClient();

        builder.Services.AddScoped<IArticleService, ArticleService>();
        builder.Services.AddScoped<IBookmarkService, BookmarkService>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<ILikeService, LikeService>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IContentScraper, ContentScraper>();
        builder.Services.AddScoped<IImageExtractor, ImageExtractor>();
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
        
        builder.Services.AddCors(opt =>
        {
            opt.AddPolicy(myCorsPolicy,
                policy =>
                {
                    policy
                        //.WithOrigins(/*"https://www.google.com", "https://bing.com",*/"https://localhost:4200/")
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
            accountService.CreateRoles(scope.ServiceProvider).Wait();
            
        }
        

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        
        app.UseCors(myCorsPolicy);
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseHangfireDashboard();
        //RecurringJob.AddOrUpdate<InactiveUserNotificationService>(service => service.NotifyInactiveUsersAsync(), Cron.Daily);


        app.MapControllers();

        app.Run();
        
       
    }
}