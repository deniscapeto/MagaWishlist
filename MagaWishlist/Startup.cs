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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using Polly;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Filters;

namespace MagaWishlist
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

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

            if (Configuration["DatabaseType"] == "mysql")
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

            ConfigureProductApi(services);

            services.AddMvc();

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
            .AddJwtBearer(x =>
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

            //SWAGGER
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "MagaWishlist API REST",
                    Version = "v1",
                    Description = "API REST"
                });

                string appPath = PlatformServices.Default.Application.ApplicationBasePath;
                string appName = PlatformServices.Default.Application.ApplicationName;
                string xmlDocPath = Path.Combine(appPath, $"{appName}.xml");

                c.IncludeXmlComments(xmlDocPath);

                c.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
                });

                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });

        }

        private void ConfigureProductApi(IServiceCollection services)
        {
            if (!int.TryParse(Configuration["ProductAPI:EventsBeforeBreaking"], out int eventsBeforeBreaking))
                eventsBeforeBreaking = 5;

            if (!double.TryParse(Configuration["ProductAPI:DurationOfBreakInMinutes"], out double durationOfBreakInMinutes))
                durationOfBreakInMinutes = 1;

            if (!int.TryParse(Configuration["ProductAPI:TimeoutInSeconds"], out int timeoutInSeconds))
                timeoutInSeconds = 5;

            //TIMEOUT AND CIRCUIT BREAKER POLICIES
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(timeoutInSeconds));
            services.AddHttpClient("product")
                .AddPolicyHandler(timeoutPolicy)
                .AddTransientHttpErrorPolicy(
                    policyBuilder =>
                    policyBuilder.CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: eventsBeforeBreaking,
                        durationOfBreak: TimeSpan.FromMinutes(durationOfBreakInMinutes)
                    )
                );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
                    "MagaWishlist");
            });
        }
    }
}
