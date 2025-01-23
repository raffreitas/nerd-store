using Scalar.AspNetCore;

namespace NSE.Identity.API.Configuration;

public static class ApiConfiguration
{
    public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();

        return services;
    }

    public static WebApplication UserApiConfiguration(this WebApplication app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.UseIdentityConfiguration();

        app.MapControllers();

        return app;
    }
}
