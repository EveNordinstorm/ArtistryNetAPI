using System.ComponentModel.DataAnnotations;

namespace ArtistryNetAPI.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string CommentText { get; set; } = string.Empty;
        public DateTime CommentDateTime { get; set; }

        public Post Post { get; set; }
        public ApplicationUser User { get; set; }
    }
}
