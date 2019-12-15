using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using WebShop.Api.Config;
using WebShop.Api.Core;
using WebShop.Api.DataAccess;
using WebShop.Api.HttpMiddleware;

namespace WebShop.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();


            services.AddEntityFrameworkNpgsql().AddDbContext<ShopDbContext>(optionsBuilder =>
            {
                optionsBuilder.EnableDetailedErrors();
                optionsBuilder.UseNpgsql(Configuration["dbConnection"]);
            });
            
            
            services.AddSingleton<IConnectionString, ConnectionString>(_ =>
                new ConnectionString(Configuration["dbConnection"]));

            services.AddTransient<ISqlConnectionFactory, SqlConnectionFactory>();

            var authOptionsSection = Configuration.GetSection("authOptions");
            services.Configure<AuthOptions>(authOptionsSection);
            var authOptions = authOptionsSection.Get<AuthOptions>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.SecretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddTransient<IPasswordProvider, PasswordProvider>();
            services.AddMediatorExt();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSerilogRequestLogging();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseMiddleware<ExceptionFilter>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = @"UI\client-app";

                if (env.IsDevelopment())
                {
                    //spa.UseProxyToSpaDevelopmentServer("http://localhost:3100");
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }

    public class AuthOptions
    {
        public string SecretKey { get; set; }
    }
}