using Tresana.Data.Entities;
using System.Data.Entity;

namespace Tresana.Data.DataAccess
{
    public class TresanaContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<Team> Teams { get; set; }
    }
}
