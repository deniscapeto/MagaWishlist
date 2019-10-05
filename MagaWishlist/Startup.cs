using System;
using System.Data;
using System.Data.SQLite;
using System.Net.Http;
using System.Text;
using MagaWishlist.Context;
using MagaWishlist.Core.Authentication.Interfaces;
using MagaWishlist.Core.Authentication.Services;
using MagaWishlist.Core.Wishlist.Interfaces;
using MagaWishlist.Core.Wishlist.Services;
using MagaWishlist.Data;
using MagaWishlist.Middlewares;
using MagaWishlist.Models;
using MagaWishlist.Rest;
using MagaWishlist.Rest.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using Polly;

namespace MagaWishlist
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
            services.AddScoped<IJwtSecurityTokenHelper, JwtSecurityTokenHelper>();

            //SERVICES
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IWishlistService, WishlistService>();

            //RESPOSITORIES
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IWishlistRepository, WishlistRepository>();

            if(Configuration["DatabaseType"] == "mysql")
            {
                services.AddTransient<IDbConnection>((sp) => new MySqlConnection(Configuration.GetConnectionString("defaultConnection")));
                services.AddDbContext<MagaWishlistContext>(options => options.UseMySQL(Configuration.GetConnectionString("defaultConnection")));
            }
            else
            {
                services.AddTransient<IDbConnection>((sp) => new SQLiteConnection("Data Source=magawish.db"));
                services.AddDbContext<MagaWishlistContext>(options => options.UseSqlite("Data Source=magawish.db"));
            }

            //REST
            services.AddScoped<IProductRest, ProductRest>();
            services.AddScoped<IHttpClientFactoryWrapper, HttpClientFactoryWrapper>();

            //TIMEOUT AND CIRCUIT BREAKER POLICIES
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(5));
            services.AddHttpClient("product")
                .AddPolicyHandler(timeoutPolicy)
                .AddTransientHttpErrorPolicy(
                    policyBuilder => 
                    policyBuilder.CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: 2,
                        durationOfBreak: TimeSpan.FromMinutes(1)
                    )
                ); 

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddLogging(logging =>
            {
                logging.AddConfiguration(Configuration.GetSection("Logging"));
                logging.AddConsole();
            });

            services.Configure<JwtAuthentication>(Configuration.GetSection("JwtAuthentication"));
            services.AddAuthentication(x => 
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            )
            .AddJwtBearer(x=> 
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JwtAuthentication:SecurityKey"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["JwtAuthentication:Audience"],
                    ValidIssuer = Configuration["JwtAuthentication:Issuer"],
                };
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseErrorLoggingMiddleware();
            }
            else
            {
                app.UseHsts();
                app.UseExceptionHandler();
                app.UseErrorLoggingMiddleware();
            }
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
