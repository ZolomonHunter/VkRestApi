using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static VkRestApi.Models.UserGroup;

namespace VkRestApi.Models
{
    public class UserState
    {
        public enum StateCodes { Active, Blocked }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public ulong Id { get; set; }
        [Required]
        [Column("code")]
        public StateCodes Code { get; set; }
        [Column("description")]
        public string? Description { get; set; }
    }
}
