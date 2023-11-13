using Microsoft.EntityFrameworkCore;
using ReservationApp.Models.Database.Entities;
using ReservationApp.Models.Entities;
using System.Data.SqlTypes;

namespace ReservationApp.Models
{
    public partial class ReservationContext : DbContext
    {
        public ReservationContext() { }

        public ReservationContext(DbContextOptions options) : base(options) 
        {

        }

        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if !USE_PSQL_DB
            optionsBuilder.UseNpgsql("Host=dpg-cir8jo5gkuvqadpgjq60-a.singapore-postgres.render.com;Port=5432;Username=admin;Password=TAuHbkJMSiq3DiSsenXiq1PLmWeNVKuT;Database=reservation_db_y81l"
                );
#else
                optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\Coding\\Microsoft Visual Studio\\Projects\\ReservationApp\\Models\\Database\\SQL_Server_Express_LocalDB\\Database.mdf;Integrated Security=True;Connect Timeout=30");
#endif
        }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
            modelBuilder.Entity<Order>().HasIndex(o => o.Id).IsUnique();
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationTime> ReservationTimes { get; set; }
    }
}