using NSE.Identity.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddIdentityConfiguration(builder.Configuration)
    .AddApiConfiguration();

var app = builder.Build();

app.UserApiConfiguration(app.Environment);

await app.RunAsync();
