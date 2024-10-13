using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Interfaces;
using ArtistryNetAPI.Utilities;
using ArtistryNetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using ArtistryNetAPI.Dto;
using Microsoft.EntityFrameworkCore;
using ArtistryNetAPI.Data;

[Route("api/posts/{postId}/comments")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ApplicationDbContext _context;

    public CommentsController(ICommentService commentService, ApplicationDbContext context)
    {
        _commentService = commentService;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(int postId, [FromBody] CommentModel model)
    {
        {
            var userIdFromToken = JwtHelper.GetUserIdFromToken(HttpContext);

            if (userIdFromToken == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var userExists = _context.Users.Any(u => u.Id == userIdFromToken);
            if (!userExists)
            {
                return Unauthorized(new { message = "Invalid user" });
            }

            var comment = new Comment
            {
                PostId = postId,
                UserId = userIdFromToken,
                CommentText = model.CommentText,
                CommentDateTime = DateTime.Now
            };

            await _commentService.AddCommentAsync(comment);

            return Ok(new { message = "Comment added successfully" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetComments(int postId)
    {
        var comments = await _commentService.GetCommentsForPostAsync(postId);
        var commentsWithUrl = comments.Select(c => new CommentDto
        {
            Id = c.Id,
            CommentText = c.CommentText,
            CommentDateTime = c.CommentDateTime,
            UserName = c.UserName,
            ProfilePhoto = Url.Content($"~/images/profiles/{Path.GetFileName(c.ProfilePhoto)}")
        }).ToList();

        return Ok(commentsWithUrl);
    }
}
