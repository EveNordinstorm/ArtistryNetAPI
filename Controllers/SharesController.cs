using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Interfaces;
using ArtistryNetAPI.Utilities;
using ArtistryNetAPI.Models;
using ArtistryNetAPI.Services;
using ArtistryNetAPI.Dto;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using ArtistryNetAPI.Data;

[Route("api/[controller]")]
[ApiController]
public class SharesController : ControllerBase
{
    private readonly IShareService _shareService;
    private readonly ApplicationDbContext _context;

    public SharesController(IShareService shareService, ApplicationDbContext context)
    {
        _shareService = shareService;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddShare([FromBody] ShareModel model)
    {
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (userIdFromToken == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            // Fetch the user to set the username for the share
            var user = await _context.Users.FindAsync(userIdFromToken);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var share = new Share
            {
                PostId = model.PostId,
                UserId = userIdFromToken,
                ShareDateTime = DateTime.Now
            };

            await _shareService.AddShareAsync(share);

            return Ok(new { id = share.Id, message = "Share added successfully" });
        }
    }

    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetSharesForPost(int postId)
    {
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (userIdFromToken == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var shares = await _shareService.GetSharesForPostAsync(postId);
            var isSharedByUser = shares.Any(share => share.UserId == userIdFromToken);

            return Ok(new { isSharedByUser });
        }
    }

    [HttpGet("share-status/{postId}")]
    public async Task<IActionResult> CheckShareStatusAsync(int postId)
    {
        var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

        if (userIdFromToken == null)
        {
            return Unauthorized(new { message = "Invalid token" });
        }

        var isShared = await _shareService.HasUserSharedPostAsync(userIdFromToken, postId);
        return Ok(new { isSharedByUser = isShared });
    }

    [HttpDelete("{postId}")]
    public async Task<IActionResult> RemoveShare(int postId)
    {
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            await _shareService.RemoveShareAsync(postId, userIdFromToken);

            return Ok(new { message = "Share removed successfully" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllShares()
    {
        {
            var shares = await _shareService.GetAllSharesAsync();

            var sharesDto = shares.Select(share => new SharesDto
            {
                Id = share.Id,
                PostId = share.PostId,
                ShareDateTime = share.ShareDateTime,
                Sharer = new SharerDto
                {
                    UserName = share.User?.UserName,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.User?.ProfilePhoto)}"),
                    UserId = share.UserId
                },
                OriginalPost = new OriginalPostDto
                {
                    Id = share.Post.Id,
                    Description = share.Post.Description,
                    ImageUrl = Url.Content($"~/images/posts/{Path.GetFileName(share.Post.ImageUrl)}"),
                    PostDateTime = share.Post.PostDateTime,
                    UserName = share.Post.UserName,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.Post.User?.ProfilePhoto)}"),
                    UserId = share.Post.UserId
                }
            });

            return Ok(sharesDto);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetShareByIdAsync(int id)
    {
        {
            var share = await _context.Shares
                .Include(s => s.Post)
                .ThenInclude(p => p.User)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (share == null) return NotFound();

            var sharesDto = new SharesDto
            {
                Id = share.Id,
                PostId = share.PostId,
                ShareDateTime = share.ShareDateTime,
                Sharer = new SharerDto
                {
                    UserName = share.User?.UserName,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.User?.ProfilePhoto)}"),
                    UserId = share.UserId
                },
                OriginalPost = new OriginalPostDto
                {
                    Id = share.Post.Id,
                    Description = share.Post.Description,
                    ImageUrl = string.IsNullOrEmpty(share.Post.ImageUrl)
                            ? null
                            : Url.Content($"~/images/posts/{Path.GetFileName(share.Post.ImageUrl)}"),
                    PostDateTime = share.Post.PostDateTime,
                    UserName = share.Post.UserName,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.Post.User?.ProfilePhoto)}"),
                    UserId = share.Post.UserId
                }
            };

            return Ok(sharesDto);
        }
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetSharesByUser()
    {
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (string.IsNullOrEmpty(userIdFromToken))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var shares = await _shareService.GetSharesByUserAsync(userIdFromToken);

            var sharesDtos = shares.Select(share => new SharesDto
            {
                Id = share.Id,
                PostId = share.PostId,
                ShareDateTime = share.ShareDateTime,
                Sharer = new SharerDto
                {
                    UserName = share.User?.UserName,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.User?.ProfilePhoto)}"),
                    UserId = share.UserId
                },
                OriginalPost = new OriginalPostDto
                {
                    Id = share.Post.Id,
                    Description = share.Post.Description,
                    ImageUrl = Url.Content($"~/images/posts/{Path.GetFileName(share.Post.ImageUrl)}"),
                    PostDateTime = share.Post.PostDateTime,
                    UserName = share.Post.UserName,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.Post.User?.ProfilePhoto)}"),
                    UserId = share.Post.UserId
                }
            });

            return Ok(sharesDtos);
        }
    }

    [HttpGet("getSharesByUserName/{username}")]
    public async Task<IActionResult> GetSharesByUserNameAsync(string username)
    {
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var shares = await _shareService.GetSharesByUserAsync(user.Id);

            var sharesDtos = shares.Select(share => new SharesDto
            {
                Id = share.Id,
                PostId = share.PostId,
                ShareDateTime = share.ShareDateTime,
                Sharer = new SharerDto
                {
                    UserName = share.User?.UserName,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.User?.ProfilePhoto)}"),
                    UserId = share.UserId
                },
                OriginalPost = new OriginalPostDto
                {
                    Id = share.Post.Id,
                    Description = share.Post.Description,
                    ImageUrl = Url.Content($"~/images/posts/{Path.GetFileName(share.Post.ImageUrl)}"),
                    PostDateTime = share.Post.PostDateTime,
                    UserName = share.Post.UserName,
                    ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(share.Post.User?.ProfilePhoto)}"),
                    UserId = share.Post.UserId
                }
            });

            return Ok(sharesDtos);
        }
    }
}