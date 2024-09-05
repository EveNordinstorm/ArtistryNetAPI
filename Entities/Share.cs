﻿namespace ArtistryNetAPI.Entities
{
    public class Share
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public DateTime ShareDateTime { get; set; }

        public Post Post { get; set; }
        public ApplicationUser User { get; set; }
    }
}