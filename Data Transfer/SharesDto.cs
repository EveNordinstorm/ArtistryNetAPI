namespace ArtistryNetAPI.Dto
{
    public class SharesDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public DateTime ShareDateTime { get; set; }
        public SharerDto Sharer { get; set; }
        public OriginalPostDto OriginalPost { get; set; }
    }

    public class SharerDto
    {
        public string Username { get; set; }
        public string ProfilePhoto { get; set; }
        public string UserId { get; set; }
    }

    public class OriginalPostDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PostDateTime { get; set; }
        public string Username { get; set; }
        public string ProfilePhoto { get; set; }
        public string UserId { get; set; }

    }
}
