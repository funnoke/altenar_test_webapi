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
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/imcrying", [Authorize(AuthenticationSchemes = "Bearer")] () =>
{
    return Results.Content("You are authorization, congratulation!!");
});

app.MapPost("/register", async (TestingworkContext db, HttpContext context, IConfiguration config) =>
{
    var request = await context.Request.ReadFromJsonAsync<RegisterRequest>();
    if (request is null)
        return Results.BadRequest($"JSON не получен");
    else
    {
        Player user = new Player
        {
            Id = Guid.NewGuid(),
            UserName = request.userName
        };

        var userManager = context.RequestServices.GetService<UserManager<Player>>();
        IdentityResult result = await userManager.CreateAsync(user, request.password);

        if (result.Succeeded is false)
            return Results.BadRequest("Registration failed.");
        else
        {
            var findUser = await db.Users.FirstOrDefaultAsync(x => x.UserName == request.userName);

            if (findUser == null)
                throw new Exception($"User {request.userName} not found, varSucceeded is {result.Succeeded}");
            else
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.UserName) };
                var tokenService = context.RequestServices.GetService<ITokenService>();
                var accessJwt = tokenService.GenerateToken(claims);
                var refreshToken = tokenService.GenerateToken(claims);
                findUser.RefreshToken = refreshToken;
                findUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(config.GetSection("Tokens:RefreshTokenValidityInDays").Get<int>());
                await db.SaveChangesAsync();
                var response = new
                {
                    accessJwt = accessJwt,
                    refreshToken = refreshToken
                };
                return Results.Json(response);
            }
        }
    }

});

app.MapPost("/authorize", async (TestingworkContext db, HttpContext context, IConfiguration config) =>
{
    var request = await context.Request.ReadFromJsonAsync<AutorizeRequest>();
    if (request is null)
        return Results.BadRequest("JSON не получен");

    var _userManager = context.RequestServices.GetService<UserManager<Player>>();
    var user = await _userManager.FindByNameAsync(request.userName);
    if (user == null)
    {
        return Results.BadRequest("Bad credentials");
    }

    var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.password);
    if (!isPasswordValid)
    {
        return Results.BadRequest("Bad credentials");
    }
    //-----------------------------------------------------------------------------------------------
    var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.UserName) };
    var tokenService = context.RequestServices.GetService<ITokenService>();
    var accessJwt = tokenService.GenerateToken(claims);
    var refreshToken = tokenService.GenerateToken(claims);
    user.RefreshToken = refreshToken;
    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(config.GetSection("Tokens:RefreshTokenValidityInDays").Get<int>());
    await db.SaveChangesAsync();
    var response = new
    {
        accessJwt = accessJwt,
        refreshToken = refreshToken
    };

    return Results.Json(response);
});

app.MapPost("/refreshtoken", async (TestingworkContext db, HttpContext context, IConfiguration config) =>
{
    var request = await context.Request.ReadFromJsonAsync<RefreshTokenRequest>();
    if (request is null)
        return Results.BadRequest("JSON не получен");
    var tokenService = context.RequestServices.GetService<ITokenService>();
    var principal = tokenService.GetPrincipalFromExpiredToken(request.accessJwt);
    if (principal == null)
    {
        return Results.BadRequest("Invalid access token or refresh token");
    }
    var userManager = context.RequestServices.GetService<UserManager<Player>>();
    var username = principal.Identity!.Name;
    var user = await userManager.FindByNameAsync(username!);
    if (user == null || user.RefreshToken != request.refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
    {
        return Results.BadRequest("Invalid access token or refresh token or time is up");
    }

    var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.UserName) };
    var accessJwt = tokenService.GenerateToken(claims);

    var response = new
    {
        accessJwt = accessJwt
    };

    return Results.Json(response);

});

app.Run();