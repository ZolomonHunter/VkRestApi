using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using VkRestApi.Models;

namespace VkRestApi.Data
{
    public class ApiContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<UserState> UserStates { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {

        }

        // For testing
        public ApiContext() : base()
        {

        }
    }
}
