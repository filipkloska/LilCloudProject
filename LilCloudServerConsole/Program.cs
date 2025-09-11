using LilCloudServerConsole.Classes;
using LilCloudServerConsole.Database;
using LilCloudServerConsole.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<JwtInterceptor>();
});

builder.Services.AddDbContext<CloudContext>();

builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<DbHandler>();
builder.Services.AddScoped<LilCloudService>();


//var jwtKey = builder.Configuration["Jwt:Key"];
//var jwtIssuer = builder.Configuration["Jwt:Issuer"];
//var jwtAudience = builder.Configuration["Jwt:Audience"];
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = jwtIssuer,
//            ValidAudience = jwtAudience,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
//            ClockSkew = TimeSpan.Zero // brak dodatkowego marginesu czasu
//        };
//    });

//builder.Services.AddAuthorization();


var app = builder.Build();

//app.UseAuthentication();
//app.UseAuthorization();

// Configure the HTTP request pipeline.
app.MapGrpcService<LilCloudService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
