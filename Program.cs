using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using altenar_test_webapi;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using altenar_test_webapi.Services;
using altenar_test_webapi.Data;
using altenar_test_webapi.Models;
using altenar_test_webapi.Controllers;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder();
builder.Services.AddControllers();
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TestingworkContext>(options => options.UseSqlServer(connection));
builder.Services.AddIdentity<Player, IdentityRole<Guid>>()
        .AddEntityFrameworkStores<TestingworkContext>()
        .AddUserManager<UserManager<Player>>()
        .AddSignInManager<SignInManager<Player>>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Tokens:Key"])),
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            }
    };
});

var app = builder.Build();
app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.Run();