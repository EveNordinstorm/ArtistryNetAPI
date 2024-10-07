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
using ArtistryNetAPI.Dto;
using Microsoft.Extensions.Logging;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IUserService userService, IConfiguration configuration, ILogger<AccountController> logger)
    {
        _userService = userService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterModel model)
    {
        _logger.LogInformation("Register endpoint hit with username: {UserName}", model.UserName);

        var result = await _userService.RegisterUserAsync(model);

        if (result is ObjectResult okResult)
        {
            var user = await _userService.FindByUserNameAsync(model.UserName);
            if (user != null)
            {
                var token = GenerateJwtToken(user);
                _logger.LogInformation("User {UserName} registered successfully", user.UserName);
                return Ok(new { Message = okResult.Value, Token = token });
            }
        }

        _logger.LogWarning("User registration failed for username: {UserName}", model.UserName);
        return BadRequest(new { Message = "User registration failed." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        _logger.LogInformation("Login attempt for username: {UserName}", model.UserName);

        var user = await _userService.FindByUserNameAsync(model.UserName);
        if (user == null || !(await _userService.CheckPasswordAsync(user, model.Password)))
        {
            _logger.LogWarning("Invalid login attempt for username: {UserName}", model.UserName);
            return Unauthorized(new { Message = "Invalid credentials" });
        }

        var profilePhotoUrl = Url.Content($"~/images/profiles/{Path.GetFileName(user.ProfilePhoto)}");

        var bannerPhotoUrl = Url.Content($"~/images/banners/{Path.GetFileName(user.BannerPhoto)}");

        var token = GenerateJwtToken(user);
        _logger.LogInformation("User {UserName} logged in successfully", user.UserName);

        return Ok(new
        {
            Token = token,
            UserName = user.UserName,
            Email = user.Email,
            ProfilePhoto = profilePhotoUrl,
            BannerPhoto = bannerPhotoUrl,
            Bio = user.Bio
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        _logger.LogInformation("Fetching user details for ID: {Id}", id);

        var user = await _userService.FindByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("User not found for ID: {Id}", id);
            return NotFound(new { Message = "User not found." });
        }

        var profilePhotoUrl = Url.Content($"~/images/profiles/{Path.GetFileName(user.ProfilePhoto)}");

        var bannerPhotoUrl = Url.Content($"~/images/banners/{Path.GetFileName(user.BannerPhoto)}");

        var userDto = new UserAccountDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            ProfilePhoto = profilePhotoUrl,
            BannerPhoto = bannerPhotoUrl,
            Bio = user.Bio
        };

        return Ok(userDto);
    }

    [HttpGet("getUserDetailsByUserName/{username}")]
    public async Task<IActionResult> GetUserDetailsByUserName(string username)
    {
        _logger.LogInformation("Fetching user details for username: {UserName}", username);

        var user = await _userService.FindByUserNameAsync(username);
        if (user == null)
        {
            _logger.LogWarning("User not found for username: {UserName}", username);
            return NotFound(new { Message = "User not found." });
        }

        var profilePhotoUrl = Url.Content($"~/images/profiles/{Path.GetFileName(user.ProfilePhoto)}");

        var bannerPhotoUrl = Url.Content($"~/images/banners/{Path.GetFileName(user.BannerPhoto)}");

        var userDto = new UserAccountDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            ProfilePhoto = profilePhotoUrl,
            BannerPhoto = bannerPhotoUrl,
            Bio = user.Bio
        };

        return Ok(userDto);
    }

    [HttpPost("updateProfile")]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto model)
    {
        var userId = Request.Query["userId"].ToString();
        if (string.IsNullOrWhiteSpace(userId))
        {
            _logger.LogWarning("User ID is required for profile update.");
            return BadRequest(new { Message = "User ID is required." });
        }

        _logger.LogInformation("Updating profile for user with ID: {UserId}", userId);

        var result = await _userService.UpdateUserProfileAsync(userId, model);
        if (result)
        {
            _logger.LogInformation("Profile updated successfully for user: {UserId}", userId);
            return Ok(new { Message = "Profile updated successfully" });
        }

        _logger.LogWarning("Failed to update profile for user: {UserId}", userId);
        return BadRequest(new { Message = "Failed to update profile" });
    }
}
