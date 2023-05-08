using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace VkRestApi.Models
{
    public class UserGroup
    {
        public enum GroupCodes {Admin, User}
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public ulong Id { get; set; }
        [Required]
        [Column("code")]
        public GroupCodes Code { get; set; }
        [Column("description")]
        public string? Description { get; set; }
    }
}
