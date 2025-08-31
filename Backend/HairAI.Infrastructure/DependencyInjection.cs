using HairAI.Application.Common.Interfaces;
using HairAI.Infrastructure.Identity;
using HairAI.Infrastructure.Persistence;
using HairAI.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HairAI.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HairAI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // Configure Identity options
        services.Configure<IdentityOptions>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;

            // User settings
            options.User.RequireUniqueEmail = true;
        });

        // Add JWT Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key")))
            };
        });

        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<ICurrentUserService, CurrentUserService>();
        services.AddTransient<IJwtService, JwtService>();
        services.AddScoped<IClinicAuthorizationService, ClinicAuthorizationService>();

        services.AddTransient<IQueueService>(provider => 
            new RabbitMqService(
                configuration.GetConnectionString("RabbitMQConnection") ?? 
                throw new ArgumentNullException("RabbitMQConnection"),
                provider.GetRequiredService<ILogger<RabbitMqService>>()));

        services.AddTransient<IPaymentGateway>(provider => 
            new PaymobService(
                configuration["Paymob:ApiKey"] ?? throw new ArgumentNullException("Paymob:ApiKey"),
                configuration["Paymob:IntegrationId"] ?? throw new ArgumentNullException("Paymob:IntegrationId"),
                provider.GetRequiredService<ILogger<PaymobService>>()));

        services.AddTransient<IEmailService>(provider => 
            new SendGridEmailService(configuration["SendGrid:ApiKey"] ?? 
                throw new ArgumentNullException("SendGrid:ApiKey"),
                provider.GetRequiredService<ILogger<SendGridEmailService>>()));

        return services;
    }
}