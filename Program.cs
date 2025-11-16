using BlogWebApplication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //configure DbContext with SQL Server
            builder.Services.AddDbContext<AppDbContext>(options =>
                //getconnection string from appsettings.json
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            //configure IdentityDbContext
            builder.Services.AddIdentity<IdentityUser,IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<AppDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Auth/Login"; //redirect to login page if not authenticated
                options.LogoutPath = "/Auth/Logout";
                options.ExpireTimeSpan = TimeSpan.FromDays(3); //cookie expiration time
                options.SlidingExpiration = true; //renew cookie on each request
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Post}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
