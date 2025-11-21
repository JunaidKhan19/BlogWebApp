using BlogWebApplication.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApplication
{
    public class Program
    {
        public static async Task Main(string[] args)
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
                options.AccessDeniedPath = "/Auth/AccessDenied"; //redirect to access denied page if authorization fails
                options.ExpireTimeSpan = TimeSpan.FromDays(3); //cookie expiration time
                options.SlidingExpiration = true; //renew cookie on each request
            });

            var app = builder.Build();

            //making scope for seeding roles and admin user
            using (var scope = app.Services.CreateScope())
            {
                //accessing services
                var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                //creating roles and admin user
                string adminEmail = "admin@gmail.com";
                string adminPassword = "Admin@123";

                //check if admin role exists
                var existingAdmin = await _roleManager.FindByNameAsync("Admin");

                //if not, create it
                if (existingAdmin == null)
                {
                    //create admin role
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                //check if admin user exists
                var existingUser = await _userManager.FindByEmailAsync(adminEmail);

                //if not, create it
                if (existingUser == null)
                {
                    //create admin user
                    var newAdminUser = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };
                    var result = await _userManager.CreateAsync(newAdminUser, adminPassword);

                    //if user creation is successful, assign admin role
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(newAdminUser, "Admin");
                    }
                }
            }

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
