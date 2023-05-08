using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static VkRestApi.Models.UserGroup;

namespace VkRestApi.Models
{
    public class UserState
    {
        public enum StateCodes { Active, Blocked }
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("code")]
        public StateCodes Code { get; set; }
        [Column("description")]
        public string? Description { get; set; }
    }
}
