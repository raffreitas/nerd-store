using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using NSE.Identity.API.Data;
using NSE.Identity.API.Extensions;

namespace NSE.Identity.API.Configuration;

public static class IdentityConfiguration
{
    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddDefaultIdentity<IdentityUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // JWT
        var jwtSettingsSection = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(jwtSettingsSection);

        var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
        var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience
                };
            });
        return services;
    }

    public static IApplicationBuilder UseIdentityConfiguration(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
