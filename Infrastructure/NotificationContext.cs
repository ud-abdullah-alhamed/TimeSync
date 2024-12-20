using Microsoft.EntityFrameworkCore;
using TimeSync.Core.model;

namespace TimeSync.Infrastructure
{
    public class NotificationContext : DbContext
    {
        public NotificationContext(DbContextOptions<NotificationContext> options) : base(options) { }

        public DbSet<Notification> Notifications { get; set; } // Changed from Tasks to Notifications

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>(entity =>
            {
                // Primary Key Configuration
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TimeSent)
                      .IsRequired();

                // Foreign Key Configuration
                entity.Property(e => e.UserId)
                      .IsRequired();
                entity.Property(e => e.TaskId)
                      .IsRequired();
                entity.Property(e => e.IsFav).IsRequired(false);
                entity.Property(e => e.message)
                      .IsRequired()
                      .HasMaxLength(100)
                       .HasColumnType("varchar(100)");
                entity.Property(e => e.response)
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");
                // Foreign Key Relationship
                entity.HasOne<User>() // Replace `User` with your actual User model
                      .WithMany() // Optional: Add navigation property if needed
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // Specify delete behavior

                entity.HasOne<Tasks>() // Replace `Task` with your actual Task model
                      .WithMany() // Optional: Add navigation property if needed
                      .HasForeignKey(e => e.TaskId)
                      .OnDelete(DeleteBehavior.Cascade); // Specify delete behavior
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
