using Microsoft.EntityFrameworkCore;
using Tibur_LabAct1.Models;

namespace Tibur_LabAct1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Students { get; set; }
        public DbSet<SitIn> SitIn { get; set; }

        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
    }
}