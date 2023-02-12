using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TasteStore.DataAccess;
using TasteStore.DataAccess.Data.Initializer;
using TasteStore.DataAccess.Data.Repository;
using TasteStore.DataAccess.Data.Repository.Interfaces;
using TasteStore.Models;
using TasteStore.Utility;
using TasteStore.Utility.Interfaces;

namespace TasteStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IEmailSender, EmailSender>();
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            services.AddScoped<IDbInitializer, DbInitializer>();

            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddControllers();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            // Credentials generated at https://developers.facebook.com/
            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = Configuration["Facebook:AppId"];
                options.AppSecret = Configuration["Facebook:AppSecret"];
            });

            // Credentials generated at https://portal.azure.com/
            services.AddAuthentication().AddMicrosoftAccount(options =>
            {
                options.ClientId = Configuration["Microsoft:ClientId"];
                options.ClientSecret = Configuration["Microsoft:ClientSecret"];
            });

            // Credentials generated at https://console.developers.google.com/ (snacks.ltda.1@gmail.com)
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = Configuration["Google:ClientId"];
                options.ClientSecret = Configuration["Google:Secret"];
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();

            dbInitializer.Initialize();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoint =>
            {
                endpoint.MapControllers();
                endpoint.MapRazorPages();
            });
        }
    }
}
