using Microsoft.EntityFrameworkCore;
using ArtistryNetAPI.Data;
using ArtistryNetAPI.Entities;
using ArtistryNetAPI.Interfaces;
using ArtistryNetAPI.Dto;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;

namespace ArtistryNetAPI.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommentService(ApplicationDbContext context, IUrlHelperFactory urlHelperFactory, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _urlHelperFactory = urlHelperFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsForPostAsync(int postId)
        {
            return await _context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    CommentText = c.CommentText,
                    CommentDateTime = c.CommentDateTime,
                    Username = c.User.UserName,
                    ProfilePhoto = c.User.ProfilePhoto // Return the raw profile photo path
                })
                .ToListAsync();
        }
    }
}
