using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Models;

namespace RaceDayAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users => Set<User>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<EventCategory> EventCategories => Set<EventCategory>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    public DbSet<Result> Results => Set<Result>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<EventCategory>().HasKey(ec => new { ec.EventId, ec.CategoryId });
        modelBuilder.Entity<EventCategory>().HasOne(ec => ec.Event).WithMany(e => e.EventCategories).HasForeignKey(ec => ec.EventId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<EventCategory>().HasOne(ec => ec.Category).WithMany(c => c.EventCategories).HasForeignKey(ec => ec.CategoryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Event>().HasOne(e => e.Organizer).WithMany().HasForeignKey(e => e.OrganizerId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrolment>().HasIndex(e => new { e.ParticipantId, e.EventId }).IsUnique();
        modelBuilder.Entity<Enrolment>().HasOne(e => e.Participant).WithMany().HasForeignKey(e => e.ParticipantId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrolment>().HasOne(e => e.Event).WithMany().HasForeignKey(e => e.EventId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrolment>().HasOne<EventCategory>().WithMany().HasForeignKey(e => new { e.EventId, e.CategoryId }).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Result>().HasIndex(r => r.EnrolmentId).IsUnique();
        modelBuilder.Entity<Result>().HasOne(r => r.Enrolment).WithMany().HasForeignKey(r => r.EnrolmentId).OnDelete(DeleteBehavior.Cascade);
        base.OnModelCreating(modelBuilder);
    }
}
