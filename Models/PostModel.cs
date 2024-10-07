namespace ArtistryNetAPI.Models
{
    public class PostModel
    {
        public string UserName { get; set; } = string.Empty;
        public string ProfilePhoto { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? ImageUrl { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
