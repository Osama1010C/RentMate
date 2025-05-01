using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentMateAPI.Data;
using RentMateAPI.Extensions;
using System.Text;

namespace RentMateAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost3000",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000")
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
            });

            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowLocalhost", builder =>
            //    {
            //        builder.WithOrigins("http://localhost:3001")
            //              .AllowAnyMethod()  
            //              .AllowAnyHeader()
            //              .AllowCredentials();
            //    });
            //});
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGenJwtAuth();


            // add dbcontext
            var connectionString = builder.Configuration.GetConnectionString("constr");
            builder.Services.AddDbContext<AppDbContext>(option =>
                option.UseSqlServer(connectionString)
            );

            // register services
            builder.Services.AddSignalR();

            builder.Services.AddDataProtection();

            builder.Services.AddApplicationServices();

            var jwtSettings = builder.Configuration.GetSection("JWT");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey)
                };
            });

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.MaxAge = TimeSpan.FromMinutes(5);
                options.Cookie.HttpOnly = false;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.None; // new line
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // new line

            });

            var app = builder.Build();

            app.UseCors("AllowLocalhost3000");
            //app.UseCors("AllowLocalhost");

            //app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<ChatHub>("/chathub");

            app.MapControllers();

            app.Run();
        }
    }
}
