using ArtistryNetAPI.Dto;
using ArtistryNetAPI.Entities;

namespace ArtistryNetAPI.Interfaces
{
    public interface ICommentService
    {
        Task AddCommentAsync(Comment comment);
        Task<IEnumerable<CommentDto>> GetCommentsForPostAsync(int postId);
    }
}
