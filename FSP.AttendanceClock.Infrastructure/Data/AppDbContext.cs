using Microsoft.EntityFrameworkCore;
using FSP.AttendanceClock.Core.Entities;

namespace FSP.AttendanceClock.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<AttendanceAudit> AttendanceAudits { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.ToTable("Usuarios");
            });

            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Attendances)
                      .HasForeignKey(e => e.UserId);
                entity.ToTable("Fichajes");
            });

            modelBuilder.Entity<AttendanceAudit>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Attendance).WithMany().HasForeignKey(e => e.AttendanceId);
                entity.HasOne(e => e.ChangedByUser).WithMany().HasForeignKey(e => e.ChangedByUserId);
                entity.ToTable("AuditoriasFichajes");
            });

            modelBuilder.Entity<SystemLog>(entity =>
            {
                entity.ToTable("RegistrosSistema");
            });
        }
    }
}
