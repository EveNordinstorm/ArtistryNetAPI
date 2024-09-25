using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ArtistryNetAPI.Entities
{
    public class Follower
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FollowID { get; set; }
        public string FollowerID { get; set; }
        public ApplicationUser FollowerUser { get; set; }
        public string FollowedID { get; set; }
        public ApplicationUser FollowedUser { get; set; }
    }
}
