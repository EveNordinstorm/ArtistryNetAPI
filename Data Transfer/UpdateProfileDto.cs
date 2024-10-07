namespace ArtistryNetAPI.Dto
{
    public class UpdateProfileDto
    {
        public IFormFile? ProfilePhoto { get; set; }
        public IFormFile? BannerPhoto { get; set; }
        public string Bio { get; set; } = string.Empty;
    }
}

