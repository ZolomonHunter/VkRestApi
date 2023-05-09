using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VkRestApi.Data;
using static VkRestApi.Models.UserGroup;

namespace VkRestApi.Models
{
    public class UserState
    {
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("code")]
        public string Code { get; set; }
        [Column("description")]
        public string Description { get; set; }

        // Default values
        public UserState()
        {
            Id = 0;
            Code = UserStateEnum.ACTIVE;
            Description = "";
        }
    }
}
