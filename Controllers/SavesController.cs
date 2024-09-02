using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Interfaces;
using ArtistryNetAPI.Utilities;
using ArtistryNetAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class SavesController : ControllerBase
{
    private readonly ISaveService _saveService;

    public SavesController(ISaveService saveService)
    {
        _saveService = saveService;
    }

    [HttpPost]
    public async Task<IActionResult> AddSave([FromBody] SaveModel model)
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding save: {ex.Message}");
            return StatusCode(500, "An error occurred while adding the save.");
        }
    }

    [HttpGet("{postId}")]
    public async Task<IActionResult> GetSavesForPost(int postId)
    {
        try
        {
            var saves = await _saveService.GetSavesForPostAsync(postId);

            var saveDtos = saves.Select(save => new
            {
                save.Id,
                save.PostId,
                save.UserId
            });

            return Ok(saveDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving saves: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving the saves.");
        }
    }
}
