
using DataAccessLayer;
using DataAccessLayer.Extensions;
using Globals.Entities;
using Globals.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Stripe;
using System.Text;

namespace Backend_DormScout
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<Backend_DormScoutContext>(options => options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("Backend_DormScoutContext")));
            builder.Services.AddIdentity<User, Role>().AddEntityFrameworkStores<Backend_DormScoutContext>();// .AddDefaultTokenProviders().AddTokenProvider<EmailTokenProvider<User>>("email"); this is needed for reset password
            builder.Services.AddPortalServices(builder.Configuration);

            // Configure strongly typed settings objects
            var appSettingsSection = builder.Configuration.GetSection("AppSettings");
            builder.Services.Configure<AppSettings>(appSettingsSection);

            // Configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        // Set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                        ClockSkew = TimeSpan.Zero
                    };
                });
            
            var app = builder.Build();

            //stripe payment
            StripeConfiguration.ApiKey = "sk_test_51OUYx7Fx5b80aMliwbowltTKVYONeWalYPNXHsiMn1bj7XQZ5dE92hwqUKEJSGWhPLClfKprD1dWfxEDXFtnHoNg00aCcdasKj";
            //StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeKey").Value;

            var options = new AccountCreateOptions
            {
                Country = "BE",
                Type = "custom",
                Capabilities = new AccountCapabilitiesOptions
                {
                    CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                    Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
                },
            };
            var service = new AccountService();
            service.Create(options);

            // Uncomment to seed data from txt files
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                SeedData.Initialize(services);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                // cors setting remove in production
                app.UseCors("AllowAnyOrigins");
                //stripe payment
                app.UseDeveloperExceptionPage();
            }

            // seriLogger
            app.Use(async (context, next) =>
            {
                // Log the request details here
                var request = context.Request;
                Log.Information($"Incoming request: {request.Method} {request.Path}");

                // Continue processing the request
                await next.Invoke();

                // Log the response details here
                var response = context.Response;
                Log.Information($"Outgoing response: {response.StatusCode}");
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}