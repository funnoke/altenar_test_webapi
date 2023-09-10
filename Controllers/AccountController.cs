using Microsoft.AspNetCore.Mvc;
using altenar_test_webapi.Services;
using altenar_test_webapi.Data;
using altenar_test_webapi.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace altenar_test_webapi.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly UserManager<Player> _userManager;
    private readonly TestingworkContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AccountController(ITokenService tokenService, TestingworkContext dbContext, UserManager<Player> userManager, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _dbContext = dbContext;
        _userManager = userManager;
        _configuration = configuration;
    }
    [HttpPost("authorize")]
    public async Task<ActionResult<AuthorizeResponse>> Authenticate([FromBody] AuthorizeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var user = await _userManager.FindByNameAsync(request.userName);
        if (user == null)
        {
            return BadRequest("Bad credentials");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.password);
        if (!isPasswordValid)
        {
            return BadRequest("Bad credentials");
        }
        //-----------------------------------------------------------------------------------------------
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.UserName) };
        var accessJwt = _tokenService.GenerateToken(claims);
        var refreshToken = _tokenService.GenerateToken(claims);
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_configuration.GetSection("Tokens:RefreshTokenValidityInDays").Get<int>());
        await _dbContext.SaveChangesAsync();

        return Ok(new AuthorizeResponse
        {
            accessJwt = accessJwt,
            refreshToken = refreshToken
        });

    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthorizeResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest($"JSON не получен");
        Player user = new Player
        {
            Id = Guid.NewGuid(),
            UserName = request.userName
        };
        IdentityResult result = await _userManager.CreateAsync(user, request.password);

        if (result.Succeeded is false)
            return BadRequest("Registration failed.");

        var findUser = await _userManager.FindByNameAsync(request.userName);

        if (findUser == null)
            throw new Exception($"User {request.userName} not found");

        return await Authenticate(new AuthorizeRequest
        {
            userName = request.userName,
            password = request.password
        });


    }

    [HttpPost("refreshtoken")]
    public async Task<ActionResult<AuthorizeResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest($"JSON не получен");

        var principal = _tokenService.GetPrincipalFromExpiredToken(request.refreshToken);
        if (principal == null)
        {
            return BadRequest("Invalid access token or refresh token");
        }
        var username = principal.Identity!.Name;
        var user = await _userManager.FindByNameAsync(username!);
        if (user == null || user.RefreshToken != request.refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return BadRequest("Invalid access token or refresh token or time is up");
        }

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.UserName) };
        var accessJwt = _tokenService.GenerateToken(claims);

        return Ok(new AuthorizeResponse
        {
            accessJwt = accessJwt,
            refreshToken = request.refreshToken
        });
    }

}