using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VkRestApi.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public ulong Id { get; set; }
        [Required]
        [Column("login")]
        public string Login { get; set; }
        [Required]
        [Column("password")]
        public string Password { get; set; }
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }
        [Column("user_group_id")]
        public UserGroup UserGroup { get; set; }
        [Column("user_state_id")]
        public UserState UserState { get; set; }


    }
}
