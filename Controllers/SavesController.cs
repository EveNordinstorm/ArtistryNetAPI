using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Interfaces;
using ArtistryNetAPI.Utilities;
using ArtistryNetAPI.Models;
using ArtistryNetAPI.Data;
using ArtistryNetAPI.Dto;
using Microsoft.EntityFrameworkCore;
using ArtistryNetAPI.Services;

[Route("api/[controller]")]
[ApiController]
public class SavesController : ControllerBase
{
    private readonly ISaveService _saveService;
    private readonly ApplicationDbContext _context;

    public SavesController(ISaveService saveService, ApplicationDbContext context)
    {
        _saveService = saveService;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddSave([FromBody] SaveModel model)
    {
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (userIdFromToken == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var save = new Save
            {
                PostId = model.PostId,
                UserId = userIdFromToken
            };

            await _saveService.AddSaveAsync(save);

            return Ok(new { message = "Save added successfully" });
        }
    }

    [HttpDelete("{postId}")]
    public async Task<IActionResult> RemoveSave(int postId)
    {
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            await _saveService.RemoveSaveAsync(postId, userIdFromToken);

            return Ok(new { message = "Save removed successfully" });
        }
    }

    [HttpGet("{postId}")]
    public async Task<IActionResult> GetSavesForPost(int postId)
    {
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (userIdFromToken == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var saves = await _saveService.GetSavesForPostAsync(postId);
            var isSavedByUser = saves.Any(save => save.UserId == userIdFromToken);

            return Ok(new { isSavedByUser });
        }
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserSavedPosts()
    {
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (userIdFromToken == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var savedPosts = await _context.Saves
                .Include(s => s.Post)
                .ThenInclude(p => p.User)
                .Where(s => s.UserId == userIdFromToken)
                .Select(s => s.Post)
                .ToListAsync();

            var postDtos = savedPosts.Select(post => new PostDto
            {
                Id = post.Id,
                UserName = post.User?.UserName,
                ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(post.User?.ProfilePhoto)}"),
                PostDateTime = post.PostDateTime,
                Description = post.Description,
                ImageUrl = Url.Content($"~/images/posts/{post.ImageUrl}"),
                UserId = post.UserId,
                IsSavedByUser = true
            });

            return Ok(postDtos);
        }
    }
}
