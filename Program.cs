using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using test.Areas.AdminPanel.Data;
using test.Data;
using test.DataAccessLayer;
using test.Models.IdentityModels;

namespace test
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddMvc().AddNewtonsoftJson(opt=> opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(
                 connectionString,
                 builder =>
                 {
                     builder.MigrationsAssembly("test");
                 }
                 ));

            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<LocalizedIdentityErrorDescriber>();

            builder.Services.AddSession(opt=>opt.IdleTimeout=TimeSpan.FromSeconds(1));
            

            Constants.RootPath = builder.Environment.WebRootPath;

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext=scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var dataInit = new DataInitializer(dbContext,userManager,roleManager);
                await dataInit.SeedData();
            }

            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Home/e_404";
                    await next();
                }
                if (context.Response.StatusCode == 403)
                {
                    context.Request.Path = "/Home/e_403";
                    await next();
                }
            });


            app.UseSession();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

                app.MapControllerRoute(
                name: "default",
                pattern: "{controller=home}/{action=index}/{id?}");
            }
            );

            app.UseStaticFiles();

            await app.RunAsync();
        }
    }
}