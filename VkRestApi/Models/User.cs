using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VkRestApi.Models
{
    public class User
    {
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("login")]
        public string Login { get; set; }
        [Required]
        [Column("password")]
        public string Password { get; set; }
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [ForeignKey("UserGroup")]
        [Column("user_group_id")]
        public int UserGroupId { get; set; }
        public UserGroup UserGroup { get; set; }

        [ForeignKey("UserState")]
        [Column("user_state_id")]
        public int UserStateId { get; set; }
        public UserState UserState { get; set; }


    }
}
