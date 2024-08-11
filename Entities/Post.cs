using System;
using System.ComponentModel.DataAnnotations;

namespace ArtistryNetAPI.Entities
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string ProfilePhoto { get; set; } = string.Empty;

        [Required]
        public DateTime PostDateTime { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
    }
}
