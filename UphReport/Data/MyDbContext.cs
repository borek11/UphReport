using Microsoft.EntityFrameworkCore;
using UphReport.Entities;
using UphReport.Entities.PageSpeedInsights;
using UphReport.Entities.Wave;

namespace UphReport.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<WebPage> WebPages { get; set; }

        public DbSet<PageSpeedReport> PageSpeedReports { get; set; }
        public DbSet<PageSpeedElement> PageSpeedElements { get; set; }
        public DbSet<PageSpeedSubElement> PageSpeedSubElements { get; set; }

        public DbSet<WaveAPIKey> WaveAPIKeys { get; set; }
        public DbSet<WaveReport> WaveReports { get; set; }
        public DbSet<WaveElement> WaveElements { get; set; }
        public DbSet<WaveSubElement> WaveSubElements { get; set; }
    }
}
