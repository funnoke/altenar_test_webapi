using Microsoft.AspNetCore.Mvc;
using altenar_test_webapi.Services;
using altenar_test_webapi.Data;
using altenar_test_webapi.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace altenar_test_webapi.Controllers;

[ApiController]
[Route("aplication")]
public class AplicationController : ControllerBase
{
    private readonly UserManager<Player> _userManager;
    private readonly TestingworkContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AplicationController(ITokenService tokenService, TestingworkContext dbContext, UserManager<Player> userManager, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _dbContext = dbContext;
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpGet("AllEvents")]
    public async Task<IActionResult> AllEventsForToday()
    {
        var applications = await _dbContext.SportsEvents
            .Where(x => (x.DateEvent >= DateTime.Today) && (x.DateEvent < DateTime.Today.AddDays(1)))
            .ToListAsync();

        return Ok(applications);
    }

    [HttpGet]
    [Route("AllEvents/{dataStart:datetime}"
                    + "/{dateEnd:datetime}")]
    public async Task<IActionResult> EventsForInterval(DateTime dataStart, DateTime dateEnd)
    {
        var applications = await _dbContext.SportsEvents
            .Where(x => (x.DateEvent >= dataStart) && (x.DateEvent < dateEnd.AddDays(1)))
            .ToListAsync();
        return Ok(applications);
    }

}