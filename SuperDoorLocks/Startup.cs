using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using SuperDoorLocks.DAL;
using SuperDoorLocks.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;

namespace SuperDoorLocks
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
            services.AddMvc();
            services.AddDbContext<DoorsDBContext>(options => 
                options.UseSqlite(Configuration.GetConnectionString("DBConnection")));
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IDoorsService, DoorsService>();
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<DoorsDBContext>();

            // JWT authentication
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = Configuration["JwtIssuer"],
                        ValidAudience = Configuration["JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            //Swagger
            services.AddSwaggerDocument(config =>
            {
                config.Title = "Super Door Locks API";
                config.Description = "Simply the best door opening solution.";
                config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT token"));

                config.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("JWT token", new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        Description = "Copy 'Bearer ' + valid JWT token into field",
                        In = OpenApiSecurityApiKeyLocation.Header

                    })
                );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, UserManager<IdentityUser> userManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            DBInitializer.SeedUsers(userManager);

            app.UseMvc();
            app.UseAuthentication();

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
