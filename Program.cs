using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentMateAPI.Services.Implementations;
using RentMateAPI.Data;
using RentMateAPI.Extensions;
using System.Text;
using RentMateAPI.Middleware;

namespace RentMateAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowVercel", builder =>
                {
                    builder.WithOrigins(
                        "https://homeless-lovat.vercel.app",
                        "http://localhost:3000"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
                });
            });
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGenJwtAuthAndXmlDoc();


            // add dbcontext
            var connectionString = builder.Configuration.GetConnectionString("constr");
            builder.Services.AddDbContext<AppDbContext>(option =>
                option.UseSqlServer(connectionString)
            );

            // register services
            builder.Services.AddSignalR();

            builder.Services.AddDataProtection();

            builder.Services.AddApplicationServices();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<JwtService>();
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
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey)
                };
            });

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(15);
                options.Cookie.MaxAge = TimeSpan.FromMinutes(15);
                options.Cookie.HttpOnly = false;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.None; // new line
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // new line

            });

            var app = builder.Build();

            app.UseCors("AllowVercel");

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
            app.MapHub<NotificationHub>("/notificationhub");

            app.UseMiddleware<ExecutionTimeLogging>();

            app.MapControllers();

            app.Run();
        }
    }
}
