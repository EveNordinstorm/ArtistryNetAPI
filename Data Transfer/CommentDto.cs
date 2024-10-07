namespace ArtistryNetAPI.Dto
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public DateTime CommentDateTime { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string ProfilePhoto { get; set; } = string.Empty;
    }
}
