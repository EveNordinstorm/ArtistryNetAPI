namespace ArtistryNetAPI.Models
{
    public class ProductModel
    {
        public string UserName { get; set; } = string.Empty;
        public string ProfilePhoto { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public IFormFile ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
