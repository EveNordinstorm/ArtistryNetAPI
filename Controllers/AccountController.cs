using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Models;
using ArtistryNetAPI.Interfaces;
using Microsoft.Extensions.Configuration;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AccountController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterModel model)
    {
        var result = await _userService.RegisterUserAsync(model);

        if (result.Succeeded)
        {
            var user = await _userService.FindByUsernameAsync(model.Username);
            if (user != null)
            {
                var token = GenerateJwtToken(user);
                return Ok(new { Message = "User registered successfully", Token = token });
            }
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userService.FindByUsernameAsync(model.Username);
        if (user == null || !(await _userService.CheckPasswordAsync(user, model.Password)))
        {
            return Unauthorized(new { Message = "Invalid credentials" });
        }

        var profilePhotoUrl = Url.Content($"~/images/profiles/{Path.GetFileName(user.ProfilePhoto)}");

        var token = GenerateJwtToken(user);
        return Ok(new
        {
            Token = token,
            Username = user.UserName,
            Email = user.Email,
            ProfilePhoto = profilePhotoUrl
        });
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
