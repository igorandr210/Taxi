using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Taxi.Data;
using Taxi.Entities;
using AutoMapper;
using Taxi.Services;
using Taxi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Taxi.Helpers;
using Taxi.Auth;
using Amazon.S3;
using Amazon.Runtime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Taxi.Hubs;
using Microsoft.Extensions.Logging;

namespace Taxi
{
    public class Startup
    {
        private IHostingEnvironment CurrentEnvironment { get; set; }   
        public Startup( IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(env.ContentRootPath)
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
              .AddEnvironmentVariables();

            //test
            // throw new Exception(AppContext.BaseDirectory);
            //throw new Exception(Directory.GetCurrentDirectory());
           // throw new Exception(env.ContentRootPath);
            Configuration = builder.Build();

            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        private string GetRDSConnectionString()
        {
            string hostname = Configuration.GetValue<string>("RDS_HOSTNAME");
            string port = Configuration.GetValue<string>("RDS_PORT");
            string dbname = Configuration.GetValue<string>("RDS_DB_NAME");
            string username = Configuration.GetValue<string>("RDS_USERNAME");
            string password = Configuration.GetValue<string>("RDS_PASSWORD");

            return "Data Source=" + hostname + ";Initial Catalog=" + dbname + ";User ID=" + username + ";Password=" + password + ";";
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var conString = Configuration.GetConnectionString("DbConnectionPost");

            if (CurrentEnvironment.IsProduction())
            {
                conString = Environment.GetEnvironmentVariable("TAXI_DB_CONN");
            }

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(conString,
                b => {
                    b.MigrationsAssembly("Taxi");
                    b.UseNetTopologySuite();
                    b.EnableRetryOnFailure(20, TimeSpan.FromMinutes(0.5),
                        errorCodesToAdd: null);
                }));
            //  services.AddScoped<ApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IDriverLocRepository, DriverLocationRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddTransient<IJwtFactory, JwtFactory>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ITripsRepository, TripsRepository>();
            services.AddScoped<IUploadService, UploadSevice>();
            services.AddScoped<IGoogleMapsService, GoogleMapsService>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper, UrlHelper>(implamantationFactory =>
            {
                var actionContext =
                    implamantationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });
            services.AddScoped<IResourceUriHelper, ResourceUriHelper>();

            services.AddSignalR();
            var awsopt = Configuration.GetAWSOptions();
            var keyId = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
           
            var creds = new BasicAWSCredentials(keyId, secretKey);
            awsopt.Credentials = creds;
            services.AddDefaultAWSOptions(awsopt);

            services.AddAWSService<IAmazonS3>();

            var keyFromConfig = Configuration["JWT_KEY"];
            if (keyFromConfig == null)
            {
                keyFromConfig = Guid.NewGuid().ToString();
            }
            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(keyFromConfig));

            var jwtOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtOptions[nameof(JwtIssuerOptions.Audience)];

                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });

            services.Configure<GoogleApiOptions>(opts =>
            {
                opts.ApiKey = Configuration["GOOGLE_API_KEY"];
            });
            services.Configure<BaseUrl>(opts =>
            {
                opts.BaseWebUrl = Configuration["BaseWebUrl"];  
            });
            
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;

                configureOptions.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // api user claim policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Customer", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.CustomerAccess));
                options.AddPolicy("Driver", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.DriverAccess));
                options.AddPolicy("DriverReg", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.DriverId));
                options.AddPolicy("Admin", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.AdminAccess));
                options.AddPolicy("Root", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.RootUserAccess));
            });

            services.Configure<EmailSenderOptions>(Configuration.GetSection("email"));

            services.AddTransient<IEmailSender, EmailSender>();


            //identity configuration
            var lockoutOptions = new LockoutOptions()
            {
                AllowedForNewUsers = true,
                DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5),
                MaxFailedAccessAttempts = 5
            };
            var builder = services.AddIdentityCore<AppUser>(o =>
            {
                // configure identity options
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
                o.Lockout = lockoutOptions;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);

            

            builder.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
      
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {Title = "Taxi api", Version = "v1"});
                c.AddSecurityDefinition("Bearer",
                    new ApiKeyScheme
                    {
                        In = "header",
                        Description = "Please enter JWT with Bearer into field",
                        Name = "Authorization",
                        Type = "apiKey"
                    });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", Enumerable.Empty<string>()}
                });
                c.OperationFilter<FileUploadOperation>();
            });
            services.AddMemoryCache();
            services.AddAutoMapper();
            services.AddCors();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
          //  app.UseHsts();
            //trying forvarde3d headers
            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
            //    ForwardedHeaders.XForwardedProto
            //});
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
          //  app.UseHttpsRedirection();
            app.UseCors(cfg =>
            {
                cfg.AllowAnyHeader().
                    AllowAnyMethod().
                    AllowAnyOrigin().
                    AllowCredentials();
            });
            //app.UseHsts();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Taxi V1");
            });

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
               
               // context.Database.Migrate();
            }

            app.UseSignalR(routes =>
            {
                routes.MapHub<RouteHub>("/route");
            });
            
            app.UseMvc(routes =>
            {
                //routes.MapRoute(
                //    name: "default",
                //    template: "{controller}/{action}/{id?}");
            });

     
        }
    }
}
// "ProfilesLocation": "credentials"
// "JWT_KEY": "LCJuYmYiOjE1MzExMzU5OTEsImV4cCI6"   