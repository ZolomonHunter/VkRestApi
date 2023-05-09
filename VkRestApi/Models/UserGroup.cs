using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using VkRestApi.Data;

namespace VkRestApi.Models
{
    public class UserGroup
    {
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("code")]
        public string Code { get; set; }
        [Column("description")]
        public string Description { get; set; }

        // Default values
        public UserGroup()
        {
            Id = 0;
            Code = UserGroupEnum.USER;
            Description = "";
        }
    }
}
