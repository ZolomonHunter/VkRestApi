using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace VkRestApi.Models
{
    public class UserGroup
    {
        public enum GroupCodes {Admin, User}
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("code")]
        public GroupCodes Code { get; set; }
        [Column("description")]
        public string Description { get; set; }

        public UserGroup()
        {
            Id = 0;
            Code = GroupCodes.User;
            Description = "";
        }
    }
}
