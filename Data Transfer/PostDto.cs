namespace ArtistryNetAPI.Dto
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string ProfilePhoto { get; set; }
        public DateTime PostDateTime { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string UserId { get; set; }
    }
}
