using Microsoft.EntityFrameworkCore;
using TimeSync.Core.model;

namespace TimeSync.Infrastructure
{
    public class TaskContext : DbContext
    {
        public TaskContext(DbContextOptions<TaskContext> options) : base(options) { }

        public DbSet<Tasks> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tasks>(entity =>
            {
                // Primary Key Configuration
                entity.HasKey(e => e.Id);

                // TaskName Configuration
                entity.Property(e => e.TaskName)
                      .IsRequired()
                      .HasMaxLength(100)
                      .HasColumnType("varchar(100)");

                // TaskDescription Configuration
                entity.Property(e => e.TaskDescription)
                      .IsRequired()
                      .HasMaxLength(100)
                      .HasColumnType("varchar(100)");

                // TaskTime Configuration
                entity.Property(e => e.TaskTime)
                      .IsRequired();

                // TaskDate Configuration
                entity.Property(e => e.TaskDate)
                      .IsRequired();

                // Period Configuration
                entity.Property(e => e.Period)
                      .IsRequired();

                // Notification Configuration
                entity.Property(e => e.Notification)
                      .IsRequired();

                // Foreign Key Configuration
                entity.Property(e => e.UserId)
                      .IsRequired();

                entity.Property(entity => entity.LastNotificationSent).IsRequired(false);

                entity.Property(entity => entity.LastUpdated).IsRequired(false);

                // Foreign Key Relationship
                entity.HasOne<User>() // Replace `User` with your actual User model
                      .WithMany() // Optional: Add navigation property if needed
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // Specify delete behavior
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
