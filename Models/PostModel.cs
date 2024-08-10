namespace ArtistryNetAPI.Models
{
    public class PostModel
    {
        public string Username { get; set; } = string.Empty;
        public string ProfilePhoto { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? ImagePath { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
